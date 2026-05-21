using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.AutomationRunner;
using NavisBOQ.Revit.Plugin.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class ExecuteRevitCodeToolHandler : IToolHandler
    {
        private readonly RevitCodeExecutor _executor;

        public ExecuteRevitCodeToolHandler()
        {
            _executor = new RevitCodeExecutor();
        }

        public string ToolName
        {
            get { return "execute_revit_code"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            JObject args = ParsePayload(request);

            UIApplication effectiveUiApp = uiApp;

            if (effectiveUiApp == null && RevitAppContext.HasUiApplication())
                effectiveUiApp = RevitAppContext.UiApplication;

            if (effectiveUiApp == null)
            {
                var fail = new RevitCodeExecutionResult
                {
                    Ok = false,
                    Mode = args.Value<string>("mode") ?? "execute",
                    Message = "UIApplication no está inicializada.",
                    Errors =
                    {
                        "No existe UIApplication disponible para ejecutar Revit API."
                    }
                };

                return ToResponse(false, "UIApplication no inicializada.", fail);
            }

            RevitCodeExecutionRequest execRequest = new RevitCodeExecutionRequest
            {
                Code = args.Value<string>("code") ?? "",
                Mode = args.Value<string>("mode") ?? "validate",
                Confirmed = args.Value<bool?>("confirmed") ?? false,
                AllowModifications = args.Value<bool?>("allow_modifications") ?? false,
                UseTransaction = args.Value<bool?>("use_transaction") ?? true,
                TransactionName = args.Value<string>("transaction_name") ?? "NavisBOQ MCP Automation",
                TimeoutMs = args.Value<int?>("timeout_ms") ?? 30000,
                ArgumentsJson = args["arguments"] != null
                    ? args["arguments"].ToString()
                    : "{}"
            };

            RevitCodeExecutionResult result = _executor.Run(effectiveUiApp, execRequest);

            return ToResponse(
                result.Ok,
                result.Message,
                result
            );
        }

        private static JObject ParsePayload(RequestEnvelope request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PayloadJson))
                return new JObject();

            return JObject.Parse(request.PayloadJson);
        }

        private static ResponseEnvelope ToResponse(
            bool ok,
            string message,
            object payload)
        {
            return new ResponseEnvelope
            {
                Ok = ok,
                Message = message ?? "",
                DataJson = JsonConvert.SerializeObject(payload),
                Error = ok ? "" : message
            };
        }
    }
}