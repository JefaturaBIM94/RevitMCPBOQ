using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;
using Newtonsoft.Json;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class ListAllowedAutomationActionsToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "list_allowed_automation_actions"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            var payload = new
            {
                feature = "Sprint 6 - Revit Automation Runner",
                status = "mvp",
                allowed_modes = new[]
                {
                    "validate",
                    "dry_run",
                    "execute"
                },
                required_code_shape =
                    "public class Command : IRevitCodeCommand { public object Execute(UIApplication uiApp, JObject args) { ... } }",
                allowed_examples = new[]
                {
                    "Leer selección activa",
                    "Leer parámetros de elementos",
                    "Modificar parámetros con Transaction",
                    "Crear cotas",
                    "Crear tags",
                    "Crear líneas de detalle",
                    "Crear TextNotes",
                    "Crear vistas",
                    "Crear sheets",
                    "Crear schedules",
                    "Mover/copiar/rotar elementos con ElementTransformUtils"
                },
                blocked_apis = new[]
                {
                    "System.IO",
                    "File",
                    "Directory",
                    "Process",
                    "Registry",
                    "System.Net",
                    "HttpClient",
                    "Reflection",
                    "Thread",
                    "Task.Run",
                    "Parallel",
                    "unsafe"
                }
            };

            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Acciones permitidas para Automation Runner.",
                DataJson = JsonConvert.SerializeObject(payload)
            };
        }
    }
}