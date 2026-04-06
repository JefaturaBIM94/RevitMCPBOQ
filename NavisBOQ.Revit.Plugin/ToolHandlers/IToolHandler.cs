using Autodesk.Revit.UI;
using NavisBOQ.Revit.Plugin.Infrastructure;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public interface IToolHandler
    {
        string ToolName { get; }
        ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request);
    }
}
