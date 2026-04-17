using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using NavisBOQ.Core.FPS;
using NavisBOQ.Core.Models;
using NavisBOQ.Core.Policies;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class RunPreconstruccion7ToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "run_preconstruccion_7"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            RunOptions options = ParseRunOptions(request != null ? request.PayloadJson : "");

            var parameterReader = new RevitParameterReaderService();
            var snapshotService = new RevitSnapshotService(parameterReader);
            var scopeService = new RevitSelectionScopeService();
            var preflightService = new RevitPreflightService(scopeService, snapshotService);
            var extractionService = new RevitSnapshotExtractionService(scopeService, snapshotService);

            var classifier = new FpsCategoryClassifierService();
            var mapper = new FpsQuantityMapperService();
            var aggregation = new FpsAggregationService();
            var executionPolicy = new ExecutionModePolicyService();

            var budget = BudgetProfiles.Corrida7;
            var readOptions = SnapshotReadOptions.ForCorrida5();

            var preflight = preflightService.BuildPreflight(
                uiApp,
                options,
                budget,
                readOptions,
                snap => MatchesOptionalCategoryFilter(snap, options));

            var execDecision = executionPolicy.EvaluateForRun("run_preconstruccion_7", options, preflight);

            if (!execDecision.AllowAutoRun)
            {
                options.OutputMode = "summary";
            }

            if (string.Equals(options.OutputMode, "auto", StringComparison.OrdinalIgnoreCase))
                options.OutputMode = preflight.ForceSummary ? "summary" : "detail";

            if (execDecision.ForceSummary)
                options.OutputMode = "summary";

            if (preflight.ForceSummary && string.Equals(options.OutputMode, "detail", StringComparison.OrdinalIgnoreCase))
                options.OutputMode = "summary";

            bool returnDetail = string.Equals(options.OutputMode, "detail", StringComparison.OrdinalIgnoreCase);

            var warnings = new List<string>();
            if (execDecision.Warnings != null && execDecision.Warnings.Count > 0)
                warnings.AddRange(execDecision.Warnings);

            var rows = new List<FpsRunRow>();
            int candidatosValidos = 0;

            var snapshots = extractionService.ExtractSnapshots(uiApp, options, readOptions);

            foreach (var snap in snapshots)
            {
                if (snap == null)
                    continue;

                if (!MatchesOptionalCategoryFilter(snap, options))
                    continue;

                if (!MatchesOptionalTypeFilter(snap, options))
                    continue;

                string boqCategory;
                string unit;

                if (!classifier.TryClassify(snap.Category, out boqCategory, out unit))
                    continue;

                candidatosValidos++;

                var row = mapper.Map(snap, boqCategory, unit);
                if (row == null)
                    continue;

                rows.Add(row);

                if (rows.Count >= budget.MaxDetailRows && returnDetail)
                {
                    warnings.Add("Detalle FPS truncado por tamaño del alcance.");
                    break;
                }
            }

            var resumen = aggregation.Aggregate(rows);

            var envelope = new ToolEnvelope<object>
            {
                Ok = true,
                Tool = "run_preconstruccion_7",
                ScopeMode = options.ScopeMode ?? "all",
                OutputMode = options.OutputMode ?? "summary",
                Preflight = preflight,
                Warnings = warnings,
                UserMessage = "Corrida 7 FPS ejecutada.",
                Data = new
                {
                    rutina = "Preconstruccion 7 - Fire Protection System",
                    ejecutado = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    total_elementos = rows.Count,
                    total_tipos = resumen.Count,
                    politica_ejecucion = new
                    {
                        modo = execDecision.Mode,
                        razon = execDecision.Reason,
                        acciones = execDecision.SuggestedActions
                    },
                    diagnostico = new
                    {
                        candidatos_validos = candidatosValidos,
                        modo = "corrida_7_fps_safe",
                        filtro_categoria = options.FilterCategory ?? "",
                        filtro_tipo = options.FilterType ?? ""
                    },
                    resumen = resumen,
                    detalle = returnDetail ? rows : null
                }
            };

            return OkJson(envelope, "Corrida 7 FPS ejecutada.");
        }

        private static RunOptions ParseRunOptions(string payloadJson)
        {
            var options = new RunOptions();

            if (string.IsNullOrWhiteSpace(payloadJson))
                return options;

            try
            {
                var node = JObject.Parse(payloadJson);

                options.ScopeMode = GetString(node, "scope_mode", options.ScopeMode);
                options.SelectionSet = GetString(node, "selection_set", options.SelectionSet);
                options.Level = GetString(node, "level", options.Level);
                options.OutputMode = GetString(node, "output_mode", options.OutputMode);
                options.MaxItems = GetInt(node, "max_items", options.MaxItems);
                options.MaxNodes = GetInt(node, "max_nodes", options.MaxNodes);
                options.StrictLimits = GetBool(node, "strict_limits", options.StrictLimits);
                options.FilterCategory = GetString(node, "filterCategory", options.FilterCategory);
                options.FilterType = GetString(node, "filterType", options.FilterType);

                return options;
            }
            catch
            {
                return options;
            }
        }

        private static bool MatchesOptionalCategoryFilter(ElementSnapshot snap, RunOptions options)
        {
            if (snap == null)
                return false;

            if (options == null || string.IsNullOrWhiteSpace(options.FilterCategory))
                return true;

            string filter = options.FilterCategory.Trim();

            if (string.Equals(filter, "fps_piping", StringComparison.OrdinalIgnoreCase))
            {
                return FpsCategoryConstants.IsPipeLike(snap.Category)
                    || FpsCategoryConstants.IsFlexPipeLike(snap.Category)
                    || FpsCategoryConstants.IsPipeFittingLike(snap.Category)
                    || FpsCategoryConstants.IsPipeAccessoryLike(snap.Category)
                    || FpsCategoryConstants.IsSprinklerLike(snap.Category)
                    || FpsCategoryConstants.IsGenericLike(snap.Category)
                    || FpsCategoryConstants.IsPlumbingFixtureLike(snap.Category)
                    || FpsCategoryConstants.IsPlumbingEquipmentLike(snap.Category);
            }

            return string.Equals((snap.Category ?? "").Trim(), filter, StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesOptionalTypeFilter(ElementSnapshot snap, RunOptions options)
        {
            if (snap == null)
                return false;

            if (options == null || string.IsNullOrWhiteSpace(options.FilterType))
                return true;

            string filter = options.FilterType.Trim();

            return ContainsIgnoreCase(snap.Type, filter)
                || ContainsIgnoreCase(snap.Family, filter)
                || ContainsIgnoreCase(snap.SystemName, filter)
                || ContainsIgnoreCase(snap.SystemClassification, filter);
        }

        private static bool ContainsIgnoreCase(string source, string value)
        {
            return !string.IsNullOrWhiteSpace(source)
                && !string.IsNullOrWhiteSpace(value)
                && source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetString(JObject node, string key, string fallback)
        {
            JToken token = node[key];
            return token != null ? (token.Value<string>() ?? fallback) : fallback;
        }

        private static int GetInt(JObject node, string key, int fallback)
        {
            JToken token = node[key];
            return token != null ? (token.Value<int?>() ?? fallback) : fallback;
        }

        private static bool GetBool(JObject node, string key, bool fallback)
        {
            JToken token = node[key];
            return token != null ? (token.Value<bool?>() ?? fallback) : fallback;
        }

        private static ResponseEnvelope OkJson(object data, string message)
        {
            return new ResponseEnvelope
            {
                Ok = true,
                Message = message,
                DataJson = JsonConvert.SerializeObject(data)
            };
        }
    }
}