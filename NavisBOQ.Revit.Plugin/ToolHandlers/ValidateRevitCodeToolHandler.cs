using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.AutomationRunner;
using NavisBOQ.Revit.Plugin.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class ValidateRevitCodeToolHandler : IToolHandler
    {
        private readonly RevitCodeSafetyValidator _validator;

        public ValidateRevitCodeToolHandler()
        {
            _validator = new RevitCodeSafetyValidator();
        }

        public string ToolName
        {
            get { return "validate_revit_code"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            JObject args = ParsePayload(request);

            RevitCodeExecutionRequest execRequest = new RevitCodeExecutionRequest
            {
                Code = args.Value<string>("code") ?? "",
                Mode = "validate",
                Confirmed = false,
                AllowModifications = args.Value<bool?>("allow_modifications") ?? false,
                UseTransaction = false,
                ArgumentsJson = args["arguments"] != null
                    ? args["arguments"].ToString()
                    : "{}"
            };

            RevitCodeExecutionResult result = _validator.Validate(execRequest);

            return new ResponseEnvelope
            {
                Ok = result.Ok,
                Message = result.Message,
                DataJson = JsonConvert.SerializeObject(result),
                Error = result.Ok ? "" : result.Message
            };
        }

        private static JObject ParsePayload(RequestEnvelope request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PayloadJson))
                return new JObject();

            return JObject.Parse(request.PayloadJson);
        }
    }
}