using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using NavisBOQ.Core.Models;
using NavisBOQ.Core.Rebar;
using NavisBOQ.Revit.Plugin.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NavisBOQ.Revit.Plugin.ToolHandlers
{
    public class RunPreconstruccion6ToolHandler : IToolHandler
    {
        public string ToolName
        {
            get { return "run_preconstruccion_6"; }
        }

        public ResponseEnvelope Handle(UIApplication uiApp, RequestEnvelope request)
        {
            var options = ParseRunOptions(request != null ? request.PayloadJson : "");

            if (uiApp == null || uiApp.ActiveUIDocument == null || uiApp.ActiveUIDocument.Document == null)
            {
                return new ResponseEnvelope
                {
                    Ok = false,
                    Message = "No hay documento activo en Revit.",
                    DataJson = "{}"
                };
            }

            var budget = BudgetProfiles.Corrida6;
            var selectedIds = uiApp.ActiveUIDocument.Selection.GetElementIds();

            var warnings = new List<string>();
            var selectedCount = selectedIds != null ? selectedIds.Count : 0;

            string outputMode = options.OutputMode ?? "auto";
            if (string.Equals(outputMode, "auto", StringComparison.OrdinalIgnoreCase))
            {
                outputMode = selectedCount > budget.GreenCandidateLimit ? "summary" : "detail";
            }

            if (selectedCount > budget.YellowCandidateLimit)
            {
                outputMode = "summary";
                warnings.Add("Corrida 6 forzada a resumen por tamaño del alcance.");
            }

            bool returnDetail = string.Equals(outputMode, "detail", StringComparison.OrdinalIgnoreCase);

            var doc = uiApp.ActiveUIDocument.Document;
            var rows = new List<RebarRunRow>();

            foreach (var id in selectedIds)
            {
                var e = doc.GetElement(id);
                if (!(e is Rebar rebar))
                    continue;

                var row = BuildRebarRow(doc, rebar, warnings);
                if (row != null)
                    rows.Add(row);

                if (returnDetail && rows.Count >= budget.MaxDetailRows)
                {
                    warnings.Add("Detalle truncado por tamaño del alcance.");
                    break;
                }
            }

            var aggregation = new RebarAggregationService();
            var resumen = aggregation.Aggregate(rows);

            var payload = new
            {
                rutina = "Preconstruccion 6 - Acero de refuerzo",
                ejecutado = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                total_instancias = rows.Count,
                total_barras = SumQuantity(rows),
                total_peso_kg = Math.Round(SumWeight(rows), 3),
                diagnostico = new
                {
                    candidatos_validos = rows.Count,
                    modo = "corrida_6_rebar_safe",
                    scope_mode = "selection",
                    output_mode = outputMode
                },
                resumen = resumen,
                detalle = returnDetail ? rows : null,
                warnings = warnings
            };

            return new ResponseEnvelope
            {
                Ok = true,
                Message = "Corrida 6 Rebar ejecutada.",
                DataJson = JsonConvert.SerializeObject(payload)
            };
        }

        private static RebarRunRow BuildRebarRow(Document doc, Rebar rebar, List<string> warnings)
        {
            if (doc == null || rebar == null)
                return null;

            var rebarType = doc.GetElement(rebar.GetTypeId()) as RebarBarType;
            if (rebarType == null)
            {
                warnings.Add("Rebar " + rebar.Id.Value + ": no se pudo resolver RebarBarType.");
                return null;
            }

            double diameterMm;
            if (!TryReadTypeBarDiameterMm(rebarType, out diameterMm))
            {
                warnings.Add("Rebar " + rebar.Id.Value + ": no se pudo leer el parámetro de tipo 'Bar Diameter'.");
                return null;
            }

            RebarBarWeightInfo weightInfo;
            if (!RebarBarWeightCatalog.TryResolveByDiameterMm(diameterMm, out weightInfo))
            {
                warnings.Add("Rebar " + rebar.Id.Value + ": diámetro " + Math.Round(diameterMm, 3) + " mm fuera del catálogo.");
                return null;
            }

            double barLengthM;
            if (!TryReadInstanceBarLengthM(rebar, out barLengthM))
            {
                warnings.Add("Rebar " + rebar.Id.Value + ": no se pudo leer el parámetro de instancia 'Bar Length'.");
                return null;
            }

            int quantity = ReadQuantity(rebar);
            if (quantity <= 0)
                quantity = 1;

            double totalLengthM = barLengthM * quantity;
            double totalWeightKg = totalLengthM * weightInfo.LinearWeightKgm;

            string shape = ReadInstanceShape(rebar);
            string levelName = ResolveLevelName(doc, rebar);

            return new RebarRunRow
            {
                Nivel = levelName,
                Categoria = "Rebar",
                Tipo = rebarType.Name ?? "",
                Shape = shape,
                BarNumber = weightInfo.BarNumber,
                DiameterMm = diameterMm,
                LinearWeightKgm = weightInfo.LinearWeightKgm,
                BarLengthM = barLengthM,
                Quantity = quantity,
                TotalLengthM = totalLengthM,
                TotalWeightKg = totalWeightKg,
                ElemId = rebar.Id.Value.ToString()
            };
        }

        private static bool TryReadTypeBarDiameterMm(RebarBarType rebarType, out double diameterMm)
        {
            diameterMm = 0.0;
            if (rebarType == null)
                return false;

            // 1) Nombre real del tipo en tu modelo: "Bar Diameter"
            var pByName = rebarType.LookupParameter("Bar Diameter");
            if (TryReadLengthParameterAsMm(pByName, out diameterMm))
                return true;

            // 2) Fallback por "Diameter"
            pByName = rebarType.LookupParameter("Diameter");
            if (TryReadLengthParameterAsMm(pByName, out diameterMm))
                return true;

            // 3) Fallback built-in
            Parameter pBuiltIn = null;

            try
            {
                pBuiltIn = rebarType.get_Parameter(BuiltInParameter.REBAR_INSTANCE_BAR_DIAMETER);
            }
            catch
            {
                pBuiltIn = null;
            }

            if (TryReadLengthParameterAsMm(pBuiltIn, out diameterMm))
                return true;

            try
            {
                pBuiltIn = rebarType.get_Parameter(BuiltInParameter.REBAR_INSTANCE_BAR_MODEL_DIAMETER);
            }
            catch
            {
                pBuiltIn = null;
            }

            if (TryReadLengthParameterAsMm(pBuiltIn, out diameterMm))
                return true;

            return false;
        }

        private static bool TryReadInstanceBarLengthM(Rebar rebar, out double barLengthM)
        {
            barLengthM = 0.0;
            if (rebar == null)
                return false;

            // 1) Nombre real del parámetro en tu modelo: "Bar Length"
            var pByName = rebar.LookupParameter("Bar Length");
            if (TryReadLengthParameterAsM(pByName, out barLengthM))
                return true;

            // 2) Fallback built-in si existiera en algunos modelos/exportaciones
            Parameter pBuiltIn = null;
            try
            {
                pBuiltIn = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_LENGTH);
            }
            catch
            {
                pBuiltIn = null;
            }

            if (TryReadLengthParameterAsM(pBuiltIn, out barLengthM))
                return true;

            return false;
        }

        private static int ReadQuantity(Rebar rebar)
        {
            if (rebar == null)
                return 1;

            // 1) Built-in Quantity of Bars
            var pBuiltIn = rebar.get_Parameter(BuiltInParameter.REBAR_ELEM_QUANTITY_OF_BARS);
            if (pBuiltIn != null && pBuiltIn.HasValue && pBuiltIn.StorageType == StorageType.Integer)
                return pBuiltIn.AsInteger();

            // 2) Fallback por nombre
            var pByName = rebar.LookupParameter("Quantity");
            if (pByName != null && pByName.HasValue && pByName.StorageType == StorageType.Integer)
                return pByName.AsInteger();

            return 1;
        }

        private static string ReadInstanceShape(Rebar rebar)
        {
            if (rebar == null)
                return "";

            // 1) Built-in
            var pBuiltIn = rebar.get_Parameter(BuiltInParameter.REBAR_SHAPE);
            var value = ReadParameterString(pBuiltIn);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            // 2) Nombre real del parámetro
            var pByName = rebar.LookupParameter("Shape");
            value = ReadParameterString(pByName);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        private static bool TryReadLengthParameterAsMm(Parameter parameter, out double valueMm)
        {
            valueMm = 0.0;

            if (parameter == null || !parameter.HasValue)
                return false;

            if (parameter.StorageType != StorageType.Double)
                return false;

            double internalValue = parameter.AsDouble();
            valueMm = UnitUtils.ConvertFromInternalUnits(internalValue, UnitTypeId.Millimeters);
            return true;
        }

        private static bool TryReadLengthParameterAsM(Parameter parameter, out double valueM)
        {
            valueM = 0.0;

            if (parameter == null || !parameter.HasValue)
                return false;

            if (parameter.StorageType != StorageType.Double)
                return false;

            double internalValue = parameter.AsDouble();
            valueM = UnitUtils.ConvertFromInternalUnits(internalValue, UnitTypeId.Meters);
            return true;
        }

        private static string ReadParameterString(Parameter parameter)
        {
            if (parameter == null || !parameter.HasValue)
                return "";

            string value = parameter.AsValueString();
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = parameter.AsString();
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        private static string ResolveLevelName(Document doc, Element element)
        {
            if (doc == null || element == null)
                return "Sin nivel";

            var level = doc.GetElement(element.LevelId) as Level;
            if (level != null)
                return level.Name ?? "Sin nivel";

            return "Sin nivel";
        }

        private static double SumWeight(List<RebarRunRow> rows)
        {
            double sum = 0.0;
            foreach (var row in rows)
                sum += row.TotalWeightKg;
            return sum;
        }

        private static int SumQuantity(List<RebarRunRow> rows)
        {
            int sum = 0;
            foreach (var row in rows)
                sum += row.Quantity;
            return sum;
        }

        private static RunOptions ParseRunOptions(string payloadJson)
        {
            var options = new RunOptions
            {
                ScopeMode = "selection",
                OutputMode = "summary"
            };

            if (string.IsNullOrWhiteSpace(payloadJson))
                return options;

            try
            {
                var node = JObject.Parse(payloadJson);
                options.ScopeMode = GetString(node, "scope_mode", options.ScopeMode);
                options.OutputMode = GetString(node, "output_mode", options.OutputMode);
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
    }
}