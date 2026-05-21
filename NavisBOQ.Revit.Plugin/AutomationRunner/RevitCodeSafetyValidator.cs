using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NavisBOQ.Revit.Plugin.AutomationRunner
{
    public class RevitCodeSafetyValidator
    {
        private static readonly string[] BlockedTokens =
        {
            "System.IO",
            "File.",
            "FileInfo",
            "Directory.",
            "DirectoryInfo",
            "Path.",
            "Process.",
            "ProcessStartInfo",
            "Microsoft.Win32",
            "Registry",
            "System.Net",
            "HttpClient",
            "WebClient",
            "Socket",
            "TcpClient",
            "UdpClient",
            "System.Reflection",
            "Assembly.Load",
            "AssemblyName",
            "Activator.CreateInstance",
            "DllImport",
            "Marshal.",
            "Environment.",
            "Thread.",
            "Task.Run",
            "Task.Factory",
            "Parallel.",
            "unsafe",
            "stackalloc"
        };

        private static readonly string[] RequiredTokens =
        {
            "IRevitCodeCommand",
            "Execute(UIApplication uiApp, JObject args)"
        };

        public RevitCodeExecutionResult Validate(RevitCodeExecutionRequest request)
        {
            var result = new RevitCodeExecutionResult
            {
                Ok = true,
                Mode = request.Mode,
                Message = "Código validado correctamente."
            };

            if (request == null)
            {
                return Fail("Request nulo.");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Fail("El código está vacío.");
            }

            foreach (var token in BlockedTokens)
            {
                if (request.Code.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Ok = false;
                    result.Errors.Add($"Token bloqueado por seguridad: {token}");
                }
            }

            foreach (var token in RequiredTokens)
            {
                if (request.Code.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    result.Ok = false;
                    result.Errors.Add($"Falta estructura requerida: {token}");
                }
            }

            if (!IsValidMode(request.Mode))
            {
                result.Ok = false;
                result.Errors.Add("Mode inválido. Usar: validate | dry_run | execute.");
            }

            if (request.Mode == "execute" && !request.Confirmed)
            {
                result.Ok = false;
                result.Errors.Add("Para ejecutar cambios reales se requiere Confirmed = true.");
            }

            if (!request.AllowModifications && LooksLikeModelModification(request.Code))
            {
                result.Ok = false;
                result.Errors.Add("El código parece modificar el modelo, pero AllowModifications = false.");
            }

            if (!result.Ok)
            {
                result.Message = "El código no pasó la validación de seguridad.";
            }

            return result;
        }

        private static bool IsValidMode(string mode)
        {
            return string.Equals(mode, "validate", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mode, "dry_run", StringComparison.OrdinalIgnoreCase)
                || string.Equals(mode, "execute", StringComparison.OrdinalIgnoreCase);
        }

        private static bool LooksLikeModelModification(string code)
        {
            var mutationTokens = new[]
            {
                ".Set(",
                "Parameter.Set",
                "doc.Create.",
                "Document.Create.",
                "NewDimension",
                "NewFamilyInstance",
                "NewDetailCurve",
                "NewTextNote",
                "ViewSheet.Create",
                "ViewSchedule.CreateSchedule",
                "ElementTransformUtils.",
                "Delete(",
                ".Delete(",
                "MoveElement",
                "CopyElement",
                "RotateElement"
            };

            return mutationTokens.Any(t => code.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static RevitCodeExecutionResult Fail(string message)
        {
            return new RevitCodeExecutionResult
            {
                Ok = false,
                Message = message,
                Errors = new List<string> { message }
            };
        }
    }
}
