using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NavisBOQ.Revit.Plugin.AutomationRunner
{
    public class RevitCodeCompiler
    {
        public RevitCodeExecutionResult Compile(string code, out Assembly compiledAssembly)
        {
            compiledAssembly = null;

            var result = new RevitCodeExecutionResult
            {
                Ok = false,
                Mode = "compile"
            };

            try
            {
                string finalCode = EnsureRequiredUsings(code);

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(finalCode);
                List<MetadataReference> references = BuildReferences();

                CSharpCompilation compilation = CSharpCompilation.Create(
                    "NavisBOQ.DynamicAutomation." + Guid.NewGuid().ToString("N"),
                    new[] { syntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

                using (MemoryStream ms = new MemoryStream())
                {
                    var emitResult = compilation.Emit(ms);

                    if (!emitResult.Success)
                    {
                        foreach (var diagnostic in emitResult.Diagnostics)
                        {
                            if (diagnostic.Severity == DiagnosticSeverity.Error)
                                result.CompilerErrors.Add(diagnostic.ToString());
                        }

                        result.Message = "Error de compilación.";
                        return result;
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    compiledAssembly = Assembly.Load(ms.ToArray());
                }

                result.Ok = true;
                result.Message = "Compilación correcta.";
                return result;
            }
            catch (Exception ex)
            {
                result.Ok = false;
                result.Message = "Error interno del compilador dinámico.";
                result.Errors.Add(ex.ToString());
                return result;
            }
        }

        private static string EnsureRequiredUsings(string code)
        {
            string preamble =
@"using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Newtonsoft.Json.Linq;
using NavisBOQ.Revit.Plugin.AutomationRunner;

";

            if (string.IsNullOrWhiteSpace(code))
                return preamble;

            if (code.Contains("NavisBOQ.Revit.Plugin.AutomationRunner"))
                return code;

            return preamble + code;
        }

        private static List<MetadataReference> BuildReferences()
        {
            var references = new List<MetadataReference>();

            AddTrustedPlatformAssemblies(references);

            AddReference(references, typeof(object).Assembly);
            AddReference(references, typeof(Enumerable).Assembly);
            AddReference(references, typeof(List<>).Assembly);

            AddReference(references, typeof(Autodesk.Revit.DB.Document).Assembly);
            AddReference(references, typeof(Autodesk.Revit.UI.UIApplication).Assembly);
            AddReference(references, typeof(Autodesk.Revit.DB.Architecture.Room).Assembly);

            AddReference(references, typeof(Newtonsoft.Json.JsonConvert).Assembly);
            AddReference(references, typeof(Newtonsoft.Json.Linq.JObject).Assembly);

            AddReference(references, typeof(IRevitCodeCommand).Assembly);

            return references
                .Where(r => r != null)
                .GroupBy(r => r.Display)
                .Select(g => g.First())
                .ToList();
        }

        private static void AddTrustedPlatformAssemblies(List<MetadataReference> references)
        {
            string tpa = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;

            if (string.IsNullOrWhiteSpace(tpa))
                return;

            foreach (string path in tpa.Split(Path.PathSeparator))
            {
                if (!File.Exists(path))
                    continue;

                references.Add(MetadataReference.CreateFromFile(path));
            }
        }

        private static void AddReference(List<MetadataReference> references, Assembly assembly)
        {
            if (assembly == null)
                return;

            if (string.IsNullOrWhiteSpace(assembly.Location))
                return;

            if (!File.Exists(assembly.Location))
                return;

            references.Add(MetadataReference.CreateFromFile(assembly.Location));
        }
    }
}