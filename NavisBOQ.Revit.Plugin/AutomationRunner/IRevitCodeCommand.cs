using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.AutomationRunner
{
    public interface IRevitCodeCommand
    {
        object Execute(UIApplication uiApp, JObject args);
    }
}