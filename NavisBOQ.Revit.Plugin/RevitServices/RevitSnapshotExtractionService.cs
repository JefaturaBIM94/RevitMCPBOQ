using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public class RevitSnapshotExtractionService
    {
        private readonly IRevitSelectionScopeService _scopeService;
        private readonly IRevitSnapshotService _snapshotService;

        public RevitSnapshotExtractionService(
            IRevitSelectionScopeService scopeService,
            IRevitSnapshotService snapshotService)
        {
            _scopeService = scopeService;
            _snapshotService = snapshotService;
        }

        public List<ElementSnapshot> ExtractSnapshots(UIApplication uiApp, RunOptions options)
        {
            var result = new List<ElementSnapshot>();

            if (uiApp == null || uiApp.ActiveUIDocument == null || uiApp.ActiveUIDocument.Document == null)
                return result;

            Document doc = uiApp.ActiveUIDocument.Document;
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var elements = _scopeService.ResolveScopeElements(uiApp, options);

            foreach (var element in elements)
            {
                var snap = _snapshotService.BuildSnapshot(doc, element);
                if (snap == null)
                    continue;

                string key = !string.IsNullOrWhiteSpace(snap.UniqueId)
                    ? snap.UniqueId
                    : snap.CanonicalId ?? "";

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                if (!seen.Add(key))
                    continue;

                result.Add(snap);
            }

            return result;
        }
    }
}