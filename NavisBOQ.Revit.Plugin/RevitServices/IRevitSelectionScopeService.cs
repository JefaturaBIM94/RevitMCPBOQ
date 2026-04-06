using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public interface IRevitSelectionScopeService
    {
        IList<Element> ResolveScopeElements(UIApplication uiApp, RunOptions options);
    }
}