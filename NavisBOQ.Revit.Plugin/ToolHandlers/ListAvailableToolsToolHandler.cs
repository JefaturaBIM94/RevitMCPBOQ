using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class ListAvailableToolsToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "list_available_tools"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            string json =
                "{ " +
                "\"tools\": [" +
                "\"ping\", " +
                "\"active_document_info\", " +
                "\"diagnose_selection\", " +
                "\"run_preconstruccion_4\", " +
                 "\"run_preconstruccion_5\", " +
                "\"expand_electrical_detail\"" +
                "]" +
                "}";

            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Listado de tools disponible.",
                DataJson = json
            };
        }
    }
}