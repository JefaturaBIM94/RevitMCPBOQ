using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public class RevitSelectionScopeService : IRevitSelectionScopeService
    {
        public IList<Element> ResolveScopeElements(UIApplication uiApp, RunOptions options)
        {
            var result = new List<Element>();

            if (uiApp == null || uiApp.ActiveUIDocument == null || uiApp.ActiveUIDocument.Document == null)
                return result;

            Document doc = uiApp.ActiveUIDocument.Document;
            string scopeMode = (options != null ? options.ScopeMode : "all");
            scopeMode = (scopeMode ?? "all").Trim().ToLowerInvariant();

            switch (scopeMode)
            {
                case "selection":
                    return ResolveCurrentSelection(uiApp, doc);

                case "level":
                    return ResolveByLevel(doc, options != null ? options.Level : "");

                case "all":
                default:
                    return ResolveAllModelElements(doc);
            }
        }

        private IList<Element> ResolveCurrentSelection(UIApplication uiApp, Document doc)
        {
            var result = new List<Element>();
            var selectedIds = uiApp.ActiveUIDocument.Selection.GetElementIds();

            foreach (var id in selectedIds)
            {
                if (id == null || id == ElementId.InvalidElementId)
                    continue;

                Element e = doc.GetElement(id);
                if (IsValidElementCandidate(e))
                    result.Add(e);
            }

            return Deduplicate(result);
        }

        private IList<Element> ResolveByLevel(Document doc, string levelName)
        {
            var result = new List<Element>();

            if (string.IsNullOrWhiteSpace(levelName))
                return result;

            Level targetLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .FirstOrDefault(x => string.Equals(
                    x.Name ?? "",
                    levelName.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (targetLevel == null)
                return result;

            var collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .ToElements();

            foreach (var e in collector)
            {
                if (!IsValidElementCandidate(e))
                    continue;

                if (BelongsToLevel(doc, e, targetLevel.Id))
                    result.Add(e);
            }

            return Deduplicate(result);
        }

        private IList<Element> ResolveAllModelElements(Document doc)
        {
            var result = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(IsValidElementCandidate)
                .ToList();

            return Deduplicate(result);
        }

        private bool BelongsToLevel(Document doc, Element element, ElementId targetLevelId)
        {
            if (doc == null || element == null || targetLevelId == null || targetLevelId == ElementId.InvalidElementId)
                return false;

            if (element.LevelId != null && element.LevelId != ElementId.InvalidElementId)
            {
                if (element.LevelId.Value == targetLevelId.Value)
                    return true;
            }

            Parameter p = element.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (p != null && p.StorageType == StorageType.ElementId)
            {
                ElementId levelId = p.AsElementId();
                if (levelId != null && levelId != ElementId.InvalidElementId && levelId.Value == targetLevelId.Value)
                    return true;
            }

            return false;
        }

        private bool IsValidElementCandidate(Element element)
        {
            if (element == null)
                return false;

            if (element.Category == null)
                return false;

            if (element is ElementType)
                return false;

            if (element.ViewSpecific)
                return false;

            return true;
        }

        private IList<Element> Deduplicate(IEnumerable<Element> elements)
        {
            var result = new List<Element>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var e in elements ?? Enumerable.Empty<Element>())
            {
                if (e == null)
                    continue;

                string key = e.UniqueId ?? (e.Id != null ? e.Id.Value.ToString() : "");
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (seen.Add(key))
                    result.Add(e);
            }

            return result;
        }
    }
}