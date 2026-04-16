using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Architecture;
using NavisBOQ.Core.Mapping;
using NavisBOQ.Core.Models;
using NavisBOQ.Core.Policies;

using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class RunPreconstruccion1ToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "run_preconstruccion_1"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            RunOptions options = ParseRunOptions(request != null ? request.PayloadJson : "");

            var parameterReader = new RevitParameterReaderService();
            var snapshotService = new RevitSnapshotService(parameterReader);
            var scopeService = new RevitSelectionScopeService();
            var preflightService = new RevitPreflightService(scopeService, snapshotService);
            var extractionService = new RevitSnapshotExtractionService(scopeService, snapshotService);

            var categoryFilter = new ArchitecturalCategoryFilterService();
            var quantityMapper = new QuantityMapperService();
            var executionPolicy = new ExecutionModePolicyService();

            var budget = BudgetProfiles.Corrida1;
            var readOptions = SnapshotReadOptions.ForCorrida1();

            var preflight = preflightService.BuildPreflight(uiApp, options, budget, readOptions);
            var execDecision = executionPolicy.EvaluateForRun("run_preconstruccion_1", options, preflight);

            if (!execDecision.AllowAutoRun)
            {
                var blockedEnvelope = new ToolEnvelope<object>
                {
                    Ok = false,
                    Tool = "run_preconstruccion_1",
                    ScopeMode = options.ScopeMode ?? "selection",
                    OutputMode = "summary",
                    Preflight = preflight,
                    Warnings = execDecision.Warnings,
                    UserMessage = execDecision.Reason,
                    Data = new
                    {
                        rutina = "Preconstruccion 1 - Arquitectura",
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
            var boqRows = new List<BoqRow>();
            int candidatosValidos = 0;

            var snapshots = extractionService.ExtractSnapshots(uiApp, options, readOptions);

            foreach (var snap in snapshots)
            {
                if (snap == null)
                    continue;

                if (!categoryFilter.IsCorrida1Category(snap.Category))
                    continue;

                candidatosValidos++;

                var row = quantityMapper.ToBoqRow(snap);
                if (row == null)
                    continue;

                boqRows.Add(row);

                if (returnDetail && boqRows.Count >= budget.MaxDetailRows)
                {
                    warnings.Add("Detalle truncado por tamaño del alcance. Reduce la selección si necesitas detalle completo.");
                    break;
                }
            }

            var resumen = boqRows
                .GroupBy(r => new
                {
                    Nivel = r.Nivel ?? "Sin nivel",
                    Categoria = r.Categoria ?? "",
                    Familia = r.Familia ?? "",
                    Tipo = r.Tipo ?? "",
                    Unidad = r.Unidad ?? ""
                })
                .Select(g => new
                {
                    Nivel = g.Key.Nivel,
                    Cat = g.Key.Categoria,
                    Familia = g.Key.Familia,
                    Tipo = g.Key.Tipo,
                    Unidad = g.Key.Unidad,
                    N = g.Count(),
                    Area = Math.Round(g.Sum(x => x.Area), 2),
                    Vol = Math.Round(g.Sum(x => x.Volumen), 2),
                    Long_ = Math.Round(g.Sum(x => x.Longitud), 2),
                    Cantidad = Math.Round(g.Sum(x => x.Cantidad), 2),
                    TipoDesc = g.Select(x => x.TipoDesc).FirstOrDefault() ?? "",
                    TipoMaterial = g.Select(x => x.TipoMaterial).FirstOrDefault() ?? "",
                    TipoAncho = Math.Round(g.Select(x => x.TipoAncho).FirstOrDefault(), 4),
                    TipoEspesor = Math.Round(g.Select(x => x.TipoEspesor).FirstOrDefault(), 4),
                    UbicacionEstructural = g.Select(x => x.UbicacionEstructural).FirstOrDefault() ?? ""
                })
                .OrderBy(x => x.Cat)
                .ThenBy(x => x.Nivel)
                .ThenBy(x => x.Tipo)
                .ToList();

            var envelope = new ToolEnvelope<object>
            {
                Ok = true,
                Tool = "run_preconstruccion_1",
                ScopeMode = options.ScopeMode ?? "selection",
                OutputMode = options.OutputMode ?? "summary",
                Preflight = preflight,
                Warnings = warnings,
                UserMessage = "Corrida 1 Arquitectura ejecutada.",
                Data = new
                {
                    rutina = "Preconstruccion 1 - Arquitectura",
                    ejecutado = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    total_elementos = boqRows.Count,
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
                        categorias = categoryFilter.GetSupportedCategories(),
                        modo = "corrida_1_revit_manual_selection"
                    },
                    resumen = resumen,
                    detalle = returnDetail ? boqRows : null
                }
            };

            return OkJson(envelope, "Corrida 1 Arquitectura ejecutada.");
        }

        private static RunOptions ParseRunOptions(string payloadJson)
        {
            var options = new RunOptions
            {
                ScopeMode = "selection",
                OutputMode = "auto"
            };

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

                if (string.IsNullOrWhiteSpace(options.ScopeMode))
                    options.ScopeMode = "selection";

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