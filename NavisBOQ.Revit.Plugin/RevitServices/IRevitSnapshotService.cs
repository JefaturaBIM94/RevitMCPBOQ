using Autodesk.Revit.DB;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public interface IRevitSnapshotService
    {
        ElementSnapshot BuildSnapshot(
            Document document,
            Element element,
            SnapshotReadOptions readOptions = null);
    }
}