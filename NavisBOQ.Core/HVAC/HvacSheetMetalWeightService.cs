using System;
using NavisBOQ.Core.Constants;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.HVAC
{
    public class HvacSheetMetalWeightService
    {
        public HvacRunRow Enrich(ElementSnapshot snap, HvacRunRow row)
        {
            if (snap == null || row == null)
                return row;

            var working = CloneAndCompleteFromSize(snap);

            row.Shape = ResolveShape(working);
            row.PressureClass = ResolvePressureClass(working);

            row.BaseInches = ResolveBaseInches(working, row.Shape);
            row.GaugeCode = ResolveGaugeCodeFromRevitRule(row.BaseInches);
            row.Gauge = "Cal " + row.GaugeCode;

            row.PerimetroM = ResolvePerimetroM(working, row.Shape);

            if (working.HasSheetMetalKgRaw)
            {
                row.KgRevitParameter = working.SheetMetalKgRaw;
                row.HasKgRevitParameter = true;
            }

            if (HvacCategoryConstants.IsDuctLike(working.Category))
                return ApplyDuctWeight(working, row);

            if (HvacCategoryConstants.IsDuctFittingLike(working.Category))
                return ApplyFittingWeight(working, row);

            return row;
        }

        private HvacRunRow ApplyDuctWeight(ElementSnapshot snap, HvacRunRow row)
        {
            double longitud = MaxNonZero(snap.LengthByInstanceM, snap.LengthM);
            row.LongitudLaminaM = longitud;

            if (row.PerimetroM > 0 && row.LongitudLaminaM > 0)
            {
                row.KgCalculated = CalculateKgByGauge(row.GaugeCode, row.PerimetroM, row.LongitudLaminaM);
                row.Kg = row.KgCalculated;
                row.KgMethod = "RevitFormulaReplica";
            }
            else
            {
                row.KgCalculated = 0.0;
                row.Kg = 0.0;
                row.KgMethod = "FallbackFactor";
            }

            return row;
        }

        private HvacRunRow ApplyFittingWeight(ElementSnapshot snap, HvacRunRow row)
        {
            string sub = (snap.FittingSubcategory ?? "").Trim();

            if (string.Equals(sub, "Transition", StringComparison.OrdinalIgnoreCase))
            {
                row.DimensionCM = ResolveTransitionDimensionCM(snap);
                if (row.PerimetroM > 0 && row.DimensionCM > 0)
                {
                    row.KgCalculated = CalculateKgByGauge(row.GaugeCode, row.PerimetroM, row.DimensionCM);
                    row.Kg = row.KgCalculated;
                    row.KgMethod = "RevitFormulaReplica_Transition";
                    return row;
                }
            }

            row.LongitudLaminaM = ResolveFittingSheetLengthM(snap, row.Shape);

            if (row.PerimetroM > 0 && row.LongitudLaminaM > 0)
            {
                row.KgCalculated = CalculateKgByGauge(row.GaugeCode, row.PerimetroM, row.LongitudLaminaM);
                row.Kg = row.KgCalculated;
                row.KgMethod = "RevitFormulaReplica_Fitting";
                return row;
            }

            row.KgCalculated = 0.0;
            row.Kg = 0.0;
            row.KgMethod = "FallbackFactor";

            return row;
        }

        private ElementSnapshot CloneAndCompleteFromSize(ElementSnapshot snap)
        {
            var c = new ElementSnapshot
            {
                Category = snap.Category,
                Type = snap.Type,
                Family = snap.Family,
                Material = snap.Material,
                TypeMaterial = snap.TypeMaterial,
                SystemClassification = snap.SystemClassification,
                SystemName = snap.SystemName,
                SystemType = snap.SystemType,

                AreaM2 = snap.AreaM2,
                LengthM = snap.LengthM,
                LengthByInstanceM = snap.LengthByInstanceM,

                SizeText = snap.SizeText,
                OverallSizeText = snap.OverallSizeText,
                FreeSizeText = snap.FreeSizeText,

                DimensionA = snap.DimensionA,
                DimensionB = snap.DimensionB,
                DimensionC = snap.DimensionC,

                DiameterM = snap.DiameterM,
                EquivalentDiameterM = snap.EquivalentDiameterM,

                DuctWidthM = snap.DuctWidthM,
                DuctHeightM = snap.DuctHeightM,
                DuctWidth1M = snap.DuctWidth1M,
                DuctHeight1M = snap.DuctHeight1M,
                DuctWidth2M = snap.DuctWidth2M,
                DuctHeight2M = snap.DuctHeight2M,
                DuctLengthM = snap.DuctLengthM,
                DuctLength1M = snap.DuctLength1M,

                WidthOffsetM = snap.WidthOffsetM,
                HeightOffsetM = snap.HeightOffsetM,
                CenterRadiusM = snap.CenterRadiusM,
                AngleDeg = snap.AngleDeg,

                FittingSubcategory = snap.FittingSubcategory,
                SheetMetalKgRaw = snap.SheetMetalKgRaw,
                HasSheetMetalKgRaw = snap.HasSheetMetalKgRaw,

                PieceBaseM = snap.PieceBaseM,
                PieceHeightM = snap.PieceHeightM,
                ReportingAngleDeg = snap.ReportingAngleDeg,
            };

            if (!string.IsNullOrWhiteSpace(c.SizeText))
            {
                var parsed = HvacFittingSizeParser.Parse(c.SizeText);

                if (parsed.Success)
                {
                    if (parsed.IsRectangular)
                    {
                        if (c.DuctWidth1M <= 0) c.DuctWidth1M = parsed.Width1M;
                        if (c.DuctHeight1M <= 0) c.DuctHeight1M = parsed.Height1M;
                        if (c.DuctWidth2M <= 0) c.DuctWidth2M = parsed.Width2M;
                        if (c.DuctHeight2M <= 0) c.DuctHeight2M = parsed.Height2M;

                        if (c.DimensionA <= 0) c.DimensionA = parsed.Width1M;
                        if (c.DimensionB <= 0) c.DimensionB = parsed.Height1M;
                    }

                    if (parsed.IsCircular)
                    {
                        if (c.DiameterM <= 0) c.DiameterM = parsed.Diameter1M;
                        if (c.EquivalentDiameterM <= 0) c.EquivalentDiameterM = parsed.Diameter2M;
                    }
                }
            }

            if (c.DuctWidthM <= 0) c.DuctWidthM = MaxNonZero(c.DuctWidth1M, c.DuctWidth2M, c.DimensionA);
            if (c.DuctHeightM <= 0) c.DuctHeightM = MaxNonZero(c.DuctHeight1M, c.DuctHeight2M, c.DimensionB);

            return c;
        }

        public string ResolveShape(ElementSnapshot snap)
        {
            if (snap == null)
                return "Unknown";

            if (snap.DiameterM > 0 || snap.EquivalentDiameterM > 0)
                return "Circular";

            if ((snap.DimensionA > 0 && snap.DimensionB > 0) ||
                (snap.DuctWidthM > 0 && snap.DuctHeightM > 0) ||
                (snap.DuctWidth1M > 0 && snap.DuctHeight1M > 0) ||
                (snap.DuctWidth2M > 0 && snap.DuctHeight2M > 0))
                return "Rectangular";

            if (!string.IsNullOrWhiteSpace(snap.SizeText))
            {
                if (snap.SizeText.Contains("x"))
                    return "Rectangular";

                return "Circular";
            }

            return "Unknown";
        }

        public string ResolvePressureClass(ElementSnapshot snap)
        {
            var c = (snap.SystemClassification ?? "").Trim();

            if (c.IndexOf("suministro", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.IndexOf("supply", StringComparison.OrdinalIgnoreCase) >= 0)
                return "B";

            if (c.IndexOf("extracción", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.IndexOf("exhaust", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.IndexOf("retorno", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.IndexOf("return", StringComparison.OrdinalIgnoreCase) >= 0)
                return "A";

            return "A";
        }

        private string ResolveGaugeCodeFromRevitRule(double baseInches)
        {
            if (baseInches <= 0) return "24";
            if (baseInches < 14.0) return "26";
            if (baseInches < 31.0) return "24";
            if (baseInches < 61.0) return "22";
            if (baseInches < 97.0) return "20";
            return "18";
        }

        private double ResolveBaseInches(ElementSnapshot snap, string shape)
        {
            double baseMeters;

            if (string.Equals(shape, "Circular", StringComparison.OrdinalIgnoreCase))
            {
                baseMeters = MaxNonZero(
                    snap.DiameterM,
                    snap.EquivalentDiameterM,
                    snap.DimensionA,
                    snap.DimensionB);
            }
            else
            {
                baseMeters = MaxNonZero(
                    snap.DimensionA,
                    snap.DimensionB,
                    snap.DuctWidthM,
                    snap.DuctHeightM,
                    snap.DuctWidth1M,
                    snap.DuctHeight1M,
                    snap.DuctWidth2M,
                    snap.DuctHeight2M);
            }

            return baseMeters / 0.0254;
        }

        private double ResolvePerimetroM(ElementSnapshot snap, string shape)
        {
            if (string.Equals(shape, "Circular", StringComparison.OrdinalIgnoreCase))
            {
                double d = MaxNonZero(
                    snap.DiameterM,
                    snap.EquivalentDiameterM);

                if (d > 0)
                    return Math.PI * d;
            }

            double w = MaxNonZero(
                snap.DuctWidthM,
                snap.DuctWidth1M,
                snap.DuctWidth2M,
                snap.DimensionA,
                snap.PieceBaseM);

            double h = MaxNonZero(
                snap.DuctHeightM,
                snap.DuctHeight1M,
                snap.DuctHeight2M,
                snap.DimensionB,
                snap.PieceHeightM);

            if (w > 0 && h > 0)
                return 2.0 * (w + h);

            return 0.0;
        }

        private double ResolveTransitionDimensionCM(ElementSnapshot snap)
        {
            if (snap.DimensionC > 0)
                return snap.DimensionC;

            if (snap.DuctLengthM > 0)
                return snap.DuctLengthM;

            if (snap.DuctLength1M > 0)
                return snap.DuctLength1M;

            if (snap.LengthByInstanceM > 0)
                return snap.LengthByInstanceM;

            return 0.0;
        }

        private double ResolveFittingSheetLengthM(ElementSnapshot snap, string shape)
        {
            string sub = (snap.FittingSubcategory ?? "").Trim();

            // 1) Elbows con radio y ángulo
            if (string.Equals(sub, "Elbow", StringComparison.OrdinalIgnoreCase))
            {
                double radius = MaxNonZero(snap.CenterRadiusM);
                double angleDeg = MaxNonZero(snap.AngleDeg, snap.ReportingAngleDeg, 90.0);

                if (radius > 0 && angleDeg > 0)
                {
                    double theta = angleDeg * Math.PI / 180.0;
                    return radius * theta;
                }
            }

            // 2) Longitudes explícitas
            double explicitLength = MaxNonZero(
                snap.DuctLengthM,
                snap.DuctLength1M,
                snap.DimensionC,
                snap.LengthByInstanceM,
                snap.LengthM);

            if (explicitLength > 0)
                return explicitLength;

            // 3) Offsets como proxy de longitud desarrollada
            double widthOffset = MaxNonZero(snap.WidthOffsetM);
            double heightOffset = MaxNonZero(snap.HeightOffsetM);

            if (widthOffset > 0 || heightOffset > 0)
            {
                double diag = Math.Sqrt((widthOffset * widthOffset) + (heightOffset * heightOffset));
                if (diag > 0)
                    return diag;
            }

            // 4) Fallback geométrico por sección
            if (string.Equals(shape, "Rectangular", StringComparison.OrdinalIgnoreCase))
            {
                double w = MaxNonZero(
                    snap.DuctWidthM,
                    snap.DuctWidth1M,
                    snap.DuctWidth2M,
                    snap.DimensionA,
                    snap.PieceBaseM);

                double h = MaxNonZero(
                    snap.DuctHeightM,
                    snap.DuctHeight1M,
                    snap.DuctHeight2M,
                    snap.DimensionB,
                    snap.PieceHeightM);

                if (w > 0 && h > 0)
                {
                    double baseDim = Math.Max(w, h);

                    // longitud equivalente mínima razonable para fitting genérico
                    // evita ceros y se aproxima mejor que dejar vacío
                    return Math.Max(baseDim * 0.60, 0.15);
                }
            }

            if (string.Equals(shape, "Circular", StringComparison.OrdinalIgnoreCase))
            {
                double d = MaxNonZero(
                    snap.DiameterM,
                    snap.EquivalentDiameterM);

                if (d > 0)
                {
                    return Math.Max(d * 0.60, 0.15);
                }
            }

            return 0.0;
        }

        private double CalculateKgByGauge(string gaugeCode, double perimeterM, double lengthM)
        {
            double factor = ResolveGaugeFactor(gaugeCode);
            if (factor <= 0 || perimeterM <= 0 || lengthM <= 0)
                return 0.0;

            return ((perimeterM * lengthM) * factor) / 0.025;
        }

        private double ResolveGaugeFactor(string gaugeCode)
        {
            switch ((gaugeCode ?? "").Trim())
            {
                case "26": return 0.154241;
                case "24": return 0.177165;
                case "22": return 0.246965;
                case "20": return 0.293758;
                case "18": return 0.386647;
                default: return 0.0;
            }
        }

        private double MaxNonZero(params double[] values)
        {
            double max = 0;
            if (values == null) return 0;

            foreach (var v in values)
                if (v > max) max = v;

            return max;
        }
    }
}