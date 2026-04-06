using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class ActiveDocumentInfoToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "active_document_info"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            Document doc = uiApp != null && uiApp.ActiveUIDocument != null
                ? uiApp.ActiveUIDocument.Document
                : null;

            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Documento activo leído correctamente.",
                DataJson = BuildActiveDocumentInfo(doc)
            };
        }

        private static string BuildActiveDocumentInfo(Document doc)
        {
            if (doc == null)
                return "{ \"hasActiveDocument\": false }";

            string title = Escape(doc.Title);
            string path = Escape(doc.PathName ?? "");

            return "{ " +
                   "\"hasActiveDocument\": true, " +
                   "\"title\": \"" + title + "\", " +
                   "\"path\": \"" + path + "\"" +
                   " }";
        }

        private static string Escape(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}