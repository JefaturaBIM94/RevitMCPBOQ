using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Electrical;
using NavisBOQ.Core.Models;
using NavisBOQ.Core.Policies;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class RunPreconstruccion4ToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "run_preconstruccion_4"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            RunOptions options = ParseRunOptions(request != null ? request.PayloadJson : "");

            var parameterReader = new RevitParameterReaderService();
            var snapshotService = new RevitSnapshotService(parameterReader);
            var scopeService = new RevitSelectionScopeService();
            var preflightService = new RevitPreflightService(scopeService, snapshotService);
            var extractionService = new RevitSnapshotExtractionService(scopeService, snapshotService);

            var classifier = new ElectricalCategoryClassifierService();
            var mapper = new ElectricalQuantityMapperService();
            var aggregation = new ElectricalAggregationService();
            var executionPolicy = new ExecutionModePolicyService();

            var budget = BudgetProfiles.Corrida4;
            var readOptions = SnapshotReadOptions.ForCorrida4();
            var preflight = preflightService.BuildPreflight(
                uiApp,
                options,
                budget,
                readOptions);
            var execDecision = executionPolicy.EvaluateForRun("run_preconstruccion_4", options, preflight);

            if (!execDecision.AllowAutoRun)
            {
                var blockedEnvelope = new ToolEnvelope<object>
                {
                    Ok = false,
                    Tool = "run_preconstruccion_4",
                    ScopeMode = options.ScopeMode ?? "all",
                    OutputMode = "summary",
                    Preflight = preflight,
                    Warnings = execDecision.Warnings,
                    UserMessage = execDecision.Reason,
                    Data = new
                    {
                        politica_ejecucion = new
                        {
                            modo = execDecision.Mode,
                            razon = execDecision.Reason,
                            acciones = execDecision.SuggestedActions
                        }
                    }
                };

                return OkJson(blockedEnvelope, execDecision.Reason);
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

            var rows = new List<ElectricalRunRow>();
            int candidatesValidos = 0;
            int missingCustomPartida = 0;
            int missingPanelName = 0;
            int missingElectricalData = 0;
            int missingSizeText = 0;

            var snapshots = extractionService.ExtractSnapshots(
                uiApp,
                options,
                readOptions);

            foreach (var snap in snapshots)
            {
                if (snap == null)
                    continue;

                string boqCategory;
                string unit;

                if (!classifier.TryClassify(snap.Category, out boqCategory, out unit))
                    continue;

                candidatesValidos++;

                if (string.IsNullOrWhiteSpace(snap.CustomPartida))
                    missingCustomPartida++;

                if (string.IsNullOrWhiteSpace(snap.PanelName))
                    missingPanelName++;

                if (string.IsNullOrWhiteSpace(snap.ElectricalData))
                    missingElectricalData++;

                if (IsTubeLikeCategory(snap.Category) && string.IsNullOrWhiteSpace(snap.SizeText))
                    missingSizeText++;

                var row = mapper.Map(snap, boqCategory, unit);
                if (row == null)
                    continue;

                rows.Add(row);

                if (rows.Count >= budget.MaxDetailRows && returnDetail)
                {
                    warnings.Add("Detalle truncado por tamaño del alcance. Reduce la selección si necesitas detalle completo.");
                    break;
                }
            }

            var resumen = aggregation.Aggregate(rows);

            if (missingCustomPartida > 0)
                warnings.Add("No se encontró 'TR3Z - Partida' en " + missingCustomPartida + " elementos. La corrida continuó sin ese dato.");

            if (missingPanelName > 0)
                warnings.Add("No se encontró 'Nombre del Panel' en " + missingPanelName + " elementos. La corrida continuó sin ese dato.");

            if (missingElectricalData > 0)
                warnings.Add("No se encontró 'Datos eléctricos' en " + missingElectricalData + " elementos. La corrida continuó sin ese dato.");

            if (missingSizeText > 0)
                warnings.Add("No se encontró 'Tamaño/Size' en " + missingSizeText + " tubos. La corrida continuó sin ese dato.");

            var envelope = new ToolEnvelope<object>
            {
                Ok = true,
                Tool = "run_preconstruccion_4",
                ScopeMode = options.ScopeMode ?? "all",
                OutputMode = options.OutputMode ?? "summary",
                Preflight = preflight,
                Warnings = warnings,
                UserMessage = BuildUserScopeMessage(preflight),
                Data = new
                {
                    rutina = "Preconstruccion 4 - Electrica",
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
                        candidatos_validos = candidatesValidos,
                        modo = "corrida_4_revit_fase1"
                    },
                    resumen = resumen,
                    detalle = returnDetail ? rows : null,
                    nota = resumen.Count == 0
                        ? "No se encontraron elementos eléctricos válidos para la corrida."
                        : "OK - " + rows.Count + " elementos eléctricos procesados"
                }
            };

            return OkJson(envelope, "Corrida 4 ejecutada.");
        }

        private static RunOptions ParseRunOptions(string payloadJson)
        {
            var options = new RunOptions();

            if (string.IsNullOrWhiteSpace(payloadJson))
                return options;

            try
            {
                var node = JObject.Parse(payloadJson);

                options.ScopeMode =
                    GetString(node, "scope_mode",
                    GetString(node, "ScopeMode", options.ScopeMode));

                options.SelectionSet =
                    GetString(node, "selection_set",
                    GetString(node, "SelectionSet", options.SelectionSet));

                options.Level =
                    GetString(node, "level",
                    GetString(node, "Level", options.Level));

                options.OutputMode =
                    GetString(node, "output_mode",
                    GetString(node, "OutputMode", options.OutputMode));

                options.MaxItems =
                    GetInt(node, "max_items",
                    GetInt(node, "MaxItems", options.MaxItems));

                options.MaxNodes =
                    GetInt(node, "max_nodes",
                    GetInt(node, "MaxNodes", options.MaxNodes));

                options.StrictLimits =
                    GetBool(node, "strict_limits",
                    GetBool(node, "StrictLimits", options.StrictLimits));

                options.FilterCategory =
                    GetString(node, "filterCategory",
                    GetString(node, "FilterCategory", options.FilterCategory));

                options.FilterType =
                    GetString(node, "filterType",
                    GetString(node, "FilterType", options.FilterType));

                if (string.IsNullOrWhiteSpace(options.ScopeMode))
                    options.ScopeMode = "all";

                if (string.IsNullOrWhiteSpace(options.OutputMode))
                    options.OutputMode = "auto";

                return options;
            }
            catch
            {
                return options;
            }
        }

        private static string GetString(JObject node, string key, string fallback)
        {
            JToken token = node[key];
            if (token == null)
                return fallback;

            string value = token.Value<string>();
            return value ?? fallback;
        }

        private static int GetInt(JObject node, string key, int fallback)
        {
            JToken token = node[key];
            if (token == null)
                return fallback;

            int value;
            return int.TryParse(token.ToString(), out value) ? value : fallback;
        }

        private static bool GetBool(JObject node, string key, bool fallback)
        {
            JToken token = node[key];
            if (token == null)
                return fallback;

            bool value;
            return bool.TryParse(token.ToString(), out value) ? value : fallback;
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

        private static string BuildUserScopeMessage(ScopePreflight preflight)
        {
            if (preflight == null)
                return "";

            if (!string.IsNullOrWhiteSpace(preflight.Message))
                return preflight.Message;

            return "Corrida ejecutada correctamente.";
        }

        private static bool IsTubeLikeCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return false;

            return string.Equals(category, "Conduits", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(category, "Conduit", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(category, "Tubos", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(category, "Tubo", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(category, "Pipes", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(category, "Pipe", StringComparison.OrdinalIgnoreCase);
        }
    }
}
