using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public class RevitPreflightService
    {
        private readonly IRevitSelectionScopeService _scopeService;
        private readonly IRevitSnapshotService _snapshotService;

        public RevitPreflightService(
            IRevitSelectionScopeService scopeService,
            IRevitSnapshotService snapshotService)
        {
            _scopeService = scopeService;
            _snapshotService = snapshotService;
        }

        public ScopePreflight BuildPreflight(
            UIApplication uiApp,
            RunOptions options,
            ExecutionBudget budget,
            SnapshotReadOptions readOptions = null,
            Func<ElementSnapshot, bool> snapshotFilter = null)
        {
            readOptions = readOptions ?? SnapshotReadOptions.ForCorrida1();
            budget = budget ?? BudgetProfiles.Corrida1;

            bool strictLimits = options == null ? true : options.StrictLimits;

            int maxNodesToVisit = budget.MaxNodesToVisit;
            if (options != null && options.MaxNodes > 0 && options.MaxNodes < maxNodesToVisit)
                maxNodesToVisit = options.MaxNodes;

            var pre = new ScopePreflight
            {
                ScopeResolved = ResolveScopeLabel(options)
            };

            if (uiApp == null || uiApp.ActiveUIDocument == null || uiApp.ActiveUIDocument.Document == null)
            {
                pre.RiskBand = "red";
                pre.AllowRun = false;
                pre.ForceSummary = true;
                pre.Message = "No hay documento activo en Revit.";
                return pre;
            }

            Document doc = uiApp.ActiveUIDocument.Document;

            var levels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int visited = 0;
            int candidates = 0;
            int geometricItems = 0;

            var elements = _scopeService.ResolveScopeElements(uiApp, options);

            foreach (var element in elements)
            {
                visited++;

                var snap = _snapshotService.BuildSnapshot(doc, element, readOptions);
                if (snap == null)
                    continue;

                geometricItems++;

                if (snapshotFilter != null && !snapshotFilter(snap))
                    continue;

                candidates++;

                if (!string.IsNullOrWhiteSpace(snap.Level))
                    levels.Add(snap.Level);

                if (!string.IsNullOrWhiteSpace(snap.Category))
                    categories.Add(snap.Category);

                if (visited >= maxNodesToVisit)
                    break;
            }

            pre.VisitedNodes = visited;
            pre.GeometricItems = geometricItems;
            pre.CandidateItems = candidates;
            pre.DistinctLevels = levels.Count;
            pre.DistinctCategories = categories.Count;

            if (candidates <= budget.GreenCandidateLimit)
            {
                pre.RiskBand = "green";
                pre.AllowRun = true;
                pre.ForceSummary = false;
                pre.Message = "El alcance es seguro para corrida completa.";
            }
            else if (candidates <= budget.YellowCandidateLimit)
            {
                pre.RiskBand = "yellow";
                pre.AllowRun = true;
                pre.ForceSummary = true;
                pre.Message = "El alcance es grande; por estabilidad se recomienda solo resumen.";
                pre.SuggestedSegmentation.Add("Segmenta por nivel.");
                pre.SuggestedSegmentation.Add("Usa selección actual más acotada.");
            }
            else
            {
                pre.RiskBand = "red";
                pre.AllowRun = !strictLimits;
                pre.ForceSummary = true;
                pre.Message = "El alcance excede el umbral seguro. Se recomienda reducir el alcance o ejecutar solo resumen.";
                pre.SuggestedSegmentation.Add("Usa selección manual.");
                pre.SuggestedSegmentation.Add("Corre por nivel.");
                pre.SuggestedSegmentation.Add("Filtra por categoría.");
            }

            return pre;
        }

        private static string ResolveScopeLabel(RunOptions options)
        {
            if (options == null)
                return "all";

            if (string.Equals(options.ScopeMode, "level", StringComparison.OrdinalIgnoreCase))
                return "level:" + (options.Level ?? "");

            return options.ScopeMode ?? "all";
        }
    }
}