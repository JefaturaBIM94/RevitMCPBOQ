using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class PingToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "ping"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Plugin Revit operativo.",
                DataJson = "{ \"pong\": true, \"host\": \"revit\", \"plugin\": \"NavisBOQ.Revit.Plugin\" }"
            };
        }
    }
}