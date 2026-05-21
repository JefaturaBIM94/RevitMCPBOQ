using System;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.AutomationRunner
{
    public class RevitCodeExecutor
    {
        private readonly RevitCodeSafetyValidator _validator;
        private readonly RevitCodeCompiler _compiler;

        public RevitCodeExecutor()
        {
            _validator = new RevitCodeSafetyValidator();
            _compiler = new RevitCodeCompiler();
        }

        public RevitCodeExecutionResult Run(UIApplication uiApp, RevitCodeExecutionRequest request)
        {
            RevitCodeExecutionResult validation = _validator.Validate(request);

            if (!validation.Ok)
                return validation;

            if (string.Equals(request.Mode, "validate", StringComparison.OrdinalIgnoreCase))
                return validation;

            Assembly assembly;
            RevitCodeExecutionResult compileResult = _compiler.Compile(request.Code, out assembly);

            if (!compileResult.Ok)
                return compileResult;

            if (string.Equals(request.Mode, "dry_run", StringComparison.OrdinalIgnoreCase))
            {
                compileResult.Mode = "dry_run";
                compileResult.Executed = false;
                compileResult.Message = "Dry run correcto: el código valida y compila, pero no se ejecutó.";
                return compileResult;
            }

            return ExecuteCompiledCommand(uiApp, request, assembly);
        }

        private RevitCodeExecutionResult ExecuteCompiledCommand(
            UIApplication uiApp,
            RevitCodeExecutionRequest request,
            Assembly assembly)
        {
            try
            {
                if (assembly == null)
                    return Fail(request.Mode, "El ensamblado compilado es nulo.");

                Type commandType = assembly
                    .GetTypes()
                    .FirstOrDefault(t =>
                        typeof(IRevitCodeCommand).IsAssignableFrom(t) &&
                        !t.IsInterface &&
                        !t.IsAbstract);

                if (commandType == null)
                    return Fail(request.Mode, "No se encontró una clase que implemente IRevitCodeCommand.");

                object instance = Activator.CreateInstance(commandType);

                IRevitCodeCommand command = instance as IRevitCodeCommand;

                if (command == null)
                    return Fail(request.Mode, "La clase encontrada no pudo convertirse a IRevitCodeCommand.");

                UIDocument uidoc = uiApp.ActiveUIDocument;
                Document doc = uidoc != null ? uidoc.Document : null;

                if (doc == null)
                    return Fail(request.Mode, "No hay documento activo en Revit.");

                JObject args = ParseArguments(request.ArgumentsJson);

                object output;

                if (request.UseTransaction && request.AllowModifications)
                {
                    using (Transaction tx = new Transaction(doc, request.TransactionName))
                    {
                        tx.Start();

                        output = command.Execute(uiApp, args);

                        tx.Commit();

                        return new RevitCodeExecutionResult
                        {
                            Ok = true,
                            Mode = request.Mode,
                            Executed = true,
                            TransactionStarted = true,
                            TransactionCommitted = true,
                            Message = "Código ejecutado correctamente con Transaction.",
                            OutputJson = JsonConvert.SerializeObject(output, Formatting.Indented)
                        };
                    }
                }

                output = command.Execute(uiApp, args);

                return new RevitCodeExecutionResult
                {
                    Ok = true,
                    Mode = request.Mode,
                    Executed = true,
                    TransactionStarted = false,
                    TransactionCommitted = false,
                    Message = "Código ejecutado correctamente sin Transaction.",
                    OutputJson = JsonConvert.SerializeObject(output, Formatting.Indented)
                };
            }
            catch (Exception ex)
            {
                return new RevitCodeExecutionResult
                {
                    Ok = false,
                    Mode = request.Mode,
                    Executed = false,
                    Message = "Error durante ejecución.",
                    Errors = { ex.ToString() }
                };
            }
        }

        private static JObject ParseArguments(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new JObject();

            return JObject.Parse(json);
        }

        private static RevitCodeExecutionResult Fail(string mode, string message)
        {
            return new RevitCodeExecutionResult
            {
                Ok = false,
                Mode = mode,
                Message = message,
                Errors = { message }
            };
        }
    }
}