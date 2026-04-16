using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Models;
using NavisBOQ.Core.Policies;
using NavisBOQ.Core.Steel;
using NavisBOQ.Revit.Plugin.Infrastructure;
using NavisBOQ.Revit.Plugin.RevitServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class RunPreconstruccion3ToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "run_preconstruccion_3"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            RunOptions options = ParseRunOptions(request != null ? request.PayloadJson : "");

            var parameterReader = new RevitParameterReaderService();
            var snapshotService = new RevitSnapshotService(parameterReader);
            var scopeService = new RevitSelectionScopeService();
            var preflightService = new RevitPreflightService(scopeService, snapshotService);

            var categoryFilter = new StructuralSteelCategoryFilterService();
            var steelWeightService = new StructuralSteelWeightService();
            var aggregationService = new StructuralSteelAggregationService();
            var executionPolicy = new ExecutionModePolicyService();

            var budget = BudgetProfiles.Corrida3;
            var readOptions = SnapshotReadOptions.ForCorrida3();

            var preflight = preflightService.BuildPreflight(uiApp, options, budget, readOptions);
            var execDecision = executionPolicy.EvaluateForRun("run_preconstruccion_3", options, preflight);

            if (!execDecision.AllowAutoRun)
            {
                var blockedEnvelope = new ToolEnvelope<object>
                {
                    Ok = false,
                    Tool = "run_preconstruccion_3",
                    ScopeMode = options.ScopeMode ?? "selection",
                    OutputMode = "summary",
                    Preflight = preflight,
                    Warnings = execDecision.Warnings,
                    UserMessage = execDecision.Reason,
                    Data = new
                    {
                        rutina = "Preconstruccion 3 - Estructura Metalica",
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
            var steelRows = new List<SteelRunRow>();

            int candidatosValidos = 0;
            int excluidosConcreto = 0;
            int metodo1 = 0;
            int metodo2 = 0;
            int metodo3 = 0;
            int metodo4 = 0;
            int metodo5 = 0;
            int sinDatos = 0;

            Document doc = uiApp.ActiveUIDocument.Document;

            var elements = scopeService.ResolveScopeElements(uiApp, options);
            if (elements == null)
                elements = new List<Element>();

            foreach (var element in elements)
            {
                if (element == null)
                    continue;

                var snap = snapshotService.BuildSnapshot(doc, element, readOptions);
                if (snap == null)
                    continue;

                if (!categoryFilter.IsCorrida3Category(snap.Category))
                    continue;

                candidatosValidos++;

                ElementType elementType = null;
                if (element.GetTypeId() != ElementId.InvalidElementId)
                    elementType = doc.GetElement(element.GetTypeId()) as ElementType;

                string material = !string.IsNullOrWhiteSpace(snap.Material)
                    ? snap.Material
                    : snap.TypeMaterial;

                if (steelWeightService.IsConcreteLike(material))
                {
                    excluidosConcreto++;
                    continue;
                }

                // No descartamos por "not steel" para evitar perder piezas válidas con metadata incompleta.

                double nominalWeightKgm = ResolveNominalWeightKgm(element, elementType, parameterReader);
                double linearWeightKgm = ResolveLinearWeightKgm(element, elementType, parameterReader);
                double linealWeightKgm = ResolveLinealWeightKgm(element, elementType, parameterReader);
                double weight2022Kg = ResolveWeight2022Kg(element, parameterReader);

                double lengthM = snap.LengthM;
                if (lengthM <= 0)
                    lengthM = ResolveLengthM(element, parameterReader);

                double volumeM3 = snap.VolumeM3;
                if (volumeM3 <= 0)
                    volumeM3 = ResolveVolumeM3(element, parameterReader);

                string metodo;
                string advertencia;

                double pesoKg = steelWeightService.ResolveWeightKg(
                    nominalWeightKgm,
                    linearWeightKgm,
                    linealWeightKgm,
                    lengthM,
                    weight2022Kg,
                    volumeM3,
                    out metodo,
                    out advertencia);

                if (metodo == "NominalWeight×Length") metodo1++;
                else if (metodo == "LinearWeight×Length") metodo2++;
                else if (metodo == "LinealWeight×Length") metodo3++;
                else if (metodo == "Weight(2022)") metodo4++;
                else if (metodo == "Vol×ρ") metodo5++;
                else sinDatos++;

                var row = new SteelRunRow
                {
                    Nivel = string.IsNullOrWhiteSpace(snap.Level) ? "Sin nivel" : snap.Level,
                    Categoria = snap.Category ?? "",
                    Familia = snap.Family ?? "",
                    Tipo = snap.Type ?? "",
                    SectionName = snap.SectionName ?? "",
                    SectionShape = snap.SectionShape ?? "",
                    CodeName = snap.CodeName ?? "",
                    MaterialEst = material ?? "",
                    Mark = snap.Mark ?? "",
                    ElemId = snap.ElementId ?? "",
                    NominalWeightKgm = Math.Round(nominalWeightKgm, 4),
                    LinearWeightKgm = Math.Round(linearWeightKgm > 0 ? linearWeightKgm : linealWeightKgm, 4),
                    LengthM = Math.Round(lengthM, 4),
                    VolumeM3 = Math.Round(volumeM3, 4),
                    Weight2022Kg = Math.Round(weight2022Kg, 4),
                    PesoKg = Math.Round(pesoKg, 2),
                    Metodo = metodo,
                    Advertencia = advertencia
                };

                steelRows.Add(row);

                if (returnDetail && steelRows.Count >= budget.MaxDetailRows)
                {
                    warnings.Add("Detalle truncado por tamaño del alcance. Reduce la selección si necesitas detalle completo.");
                    break;
                }
            }

            var resumen = aggregationService.Aggregate(steelRows);
            double pesoTotalKg = Math.Round(steelRows.Sum(x => x.PesoKg), 2);

            var envelope = new ToolEnvelope<object>
            {
                Ok = true,
                Tool = "run_preconstruccion_3",
                ScopeMode = options.ScopeMode ?? "selection",
                OutputMode = options.OutputMode ?? "summary",
                Preflight = preflight,
                Warnings = warnings,
                UserMessage = "Corrida 3 Estructura Metalica ejecutada.",
                Data = new
                {
                    rutina = "Preconstruccion 3 - Estructura Metalica Estandar 2025+",
                    ejecutado = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    total_elementos = steelRows.Count,
                    total_tipos = resumen.Count,
                    peso_total_kg = pesoTotalKg,
                    peso_total_ton = pesoTotalKg >= 1000.0 ? Math.Round(pesoTotalKg / 1000.0, 3) : (double?)null,
                    politica_ejecucion = new
                    {
                        modo = execDecision.Mode,
                        razon = execDecision.Reason,
                        acciones = execDecision.SuggestedActions
                    },
                    diagnostico = new
                    {
                        candidatos_validos = candidatosValidos,
                        excluidos_concreto = excluidosConcreto,
                        metodo_nominal_weight = metodo1,
                        metodo_linear_weight = metodo2,
                        metodo_lineal_weight = metodo3,
                        metodo_weight_2022 = metodo4,
                        metodo_fallback = metodo5,
                        sin_datos = sinDatos,
                        categorias = categoryFilter.GetSupportedCategories(),
                        modo = "corrida_3_revit_manual_selection"
                    },
                    resumen = resumen,
                    detalle = returnDetail ? steelRows : null
                }
            };

            return OkJson(envelope, "Corrida 3 Estructura Metalica ejecutada.");
        }

        private static double ResolveNominalWeightKgm(Element element, ElementType elementType, RevitParameterReaderService reader)
        {
            if (elementType != null)
            {
                double value;
                if (reader.TryReadDisplayDoubleByParameterName(elementType, "Nominal Weight", out value) && value > 0)
                    return value;
            }

            // Fallback al valor ya resuelto por snapshot/type reader
            if (elementType != null)
            {
                try
                {
                    return reader.ReadNominalWeightKgm(element, elementType);
                }
                catch
                {
                    return 0.0;
                }
            }

            return 0.0;
        }

        private static double ResolveLinearWeightKgm(Element element, ElementType elementType, RevitParameterReaderService reader)
        {
            if (elementType != null)
            {
                double value;
                if (reader.TryReadDisplayDoubleByParameterName(elementType, "Linear Weight", out value) && value > 0)
                    return value;

                if (reader.TryReadDisplayDoubleByParameterName(elementType, "Linear Weight (kg/m)", out value) && value > 0)
                    return value;
            }

            return 0.0;
        }

        private static double ResolveLinealWeightKgm(Element element, ElementType elementType, RevitParameterReaderService reader)
        {
            if (elementType != null)
            {
                double value;
                if (reader.TryReadDisplayDoubleByParameterName(elementType, "Lineal Weight", out value) && value > 0)
                    return value;

                if (reader.TryReadDisplayDoubleByParameterName(elementType, "Lineal Weight (kg/m)", out value) && value > 0)
                    return value;
            }

            return 0.0;
        }

        private static double ResolveWeight2022Kg(Element element, RevitParameterReaderService reader)
        {
            double value;
            if (reader.TryReadDisplayDoubleByParameterName(element, "Weight", out value) && value > 0)
                return value;

            return 0.0;
        }

        private static double ResolveLengthM(Element element, RevitParameterReaderService reader)
        {
            double value;
            if (reader.TryReadLengthMByParameterName(element, "Length", out value) && value > 0)
                return value;

            return 0.0;
        }

        private static double ResolveVolumeM3(Element element, RevitParameterReaderService reader)
        {
            double value;
            if (reader.TryReadVolumeM3ByParameterName(element, "Volume", out value) && value > 0)
                return value;

            return 0.0;
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