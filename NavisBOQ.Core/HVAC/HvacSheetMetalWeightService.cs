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

            if (HvacCategoryConstants.IsDuctLike(snap.Category))
                return ApplyDuctWeight(snap, row);

            if (HvacCategoryConstants.IsDuctFittingLike(snap.Category))
                return ApplyFittingWeight(snap, row);

            return row;
        }

        private HvacRunRow ApplyDuctWeight(ElementSnapshot snap, HvacRunRow row)
        {
            row.Shape = ResolveShape(snap);
            row.PressureClass = ResolvePressureClass(snap);
            row.Gauge = ResolveGauge(row.Shape, snap, row.PressureClass);
            row.ThicknessMm = GaugeToThicknessMm(row.Gauge);
            row.DensityKgM3 = ResolveDensity(snap);

            double area = snap.AreaM2 > 0 ? snap.AreaM2 : EstimateDuctArea(snap, row.Shape);
            row.AreaM2 = area > 0 ? area : 0.0;

            if (row.AreaM2 > 0 && row.ThicknessMm > 0 && row.DensityKgM3 > 0)
            {
                row.Kg = row.AreaM2 * (row.ThicknessMm / 1000.0) * row.DensityKgM3;
                row.KgMethod = "DirectArea";
            }

            return row;
        }

        private HvacRunRow ApplyFittingWeight(ElementSnapshot snap, HvacRunRow row)
        {
            row.Shape = ResolveShape(snap);
            row.PressureClass = ResolvePressureClass(snap);
            row.Gauge = ResolveGauge(row.Shape, snap, row.PressureClass);
            row.ThicknessMm = GaugeToThicknessMm(row.Gauge);
            row.DensityKgM3 = ResolveDensity(snap);

            double area = 0.0;

            if (snap.AreaM2 > 0)
            {
                area = snap.AreaM2;
                row.KgMethod = "DirectArea";
            }
            else
            {
                area = EstimateFittingArea(snap, row.Shape, out string method);
                row.KgMethod = method;
            }

            row.AreaM2 = area > 0 ? area : 0.0;

            if (row.AreaM2 > 0 && row.ThicknessMm > 0 && row.DensityKgM3 > 0)
            {
                row.Kg = row.AreaM2 * (row.ThicknessMm / 1000.0) * row.DensityKgM3;
            }

            return row;
        }

        public string ResolveShape(ElementSnapshot snap)
        {
            if (snap == null)
                return "Unknown";

            if (snap.DimensionA > 0 && snap.DimensionB > 0)
                return "Rectangular";

            if (snap.SizeText != null && snap.SizeText.Contains("x"))
                return "Rectangular";

            if (!string.IsNullOrWhiteSpace(snap.SizeText))
                return "Circular";

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

            if (c.IndexOf("aire exterior", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.IndexOf("outside air", StringComparison.OrdinalIgnoreCase) >= 0)
                return "B";

            return "A";
        }

        public string ResolveGauge(string shape, ElementSnapshot snap, string pressureClass)
        {
            double majorMm = ResolveMajorDimensionMm(snap, shape);

            if (majorMm <= 0)
                return "Cal 24";

            if (string.Equals(shape, "Circular", StringComparison.OrdinalIgnoreCase))
            {
                if (majorMm <= 500) return pressureClass == "A" ? "Cal 26" : "Cal 24";
                if (majorMm <= 900) return pressureClass == "A" ? "Cal 24" : "Cal 22";
                if (majorMm <= 1200) return pressureClass == "A" ? "Cal 22" : "Cal 20";
                return "Cal 18";
            }

            if (majorMm <= 300) return pressureClass == "A" ? "Cal 26" : "Cal 24";
            if (majorMm <= 450) return pressureClass == "A" ? "Cal 26" : "Cal 24";
            if (majorMm <= 750) return pressureClass == "A" ? "Cal 24" : "Cal 22";
            if (majorMm <= 1200) return pressureClass == "A" ? "Cal 22" : "Cal 20";
            if (majorMm <= 1800) return pressureClass == "A" ? "Cal 20" : "Cal 18";

            return "Cal 16";
        }

        public double GaugeToThicknessMm(string gauge)
        {
            switch ((gauge ?? "").Trim())
            {
                case "Cal 26": return 0.48;
                case "Cal 24": return 0.61;
                case "Cal 22": return 0.79;
                case "Cal 20": return 0.95;
                case "Cal 18": return 1.21;
                case "Cal 16": return 1.52;
                default: return 0.61;
            }
        }

        public double ResolveDensity(ElementSnapshot snap)
        {
            var mat = ((snap.TypeMaterial ?? "") + " " + (snap.Material ?? "")).Trim();

            if (mat.IndexOf("aluminum", StringComparison.OrdinalIgnoreCase) >= 0 ||
                mat.IndexOf("aluminio", StringComparison.OrdinalIgnoreCase) >= 0)
                return 2700.0;

            if (mat.IndexOf("stainless", StringComparison.OrdinalIgnoreCase) >= 0 ||
                mat.IndexOf("inox", StringComparison.OrdinalIgnoreCase) >= 0)
                return 8000.0;

            return 7850.0;
        }

        private double EstimateDuctArea(ElementSnapshot snap, string shape)
        {
            if (string.Equals(shape, "Rectangular", StringComparison.OrdinalIgnoreCase))
            {
                if (snap.DimensionA > 0 && snap.DimensionB > 0 && snap.LengthByInstanceM > 0)
                {
                    double perimeter = 2.0 * (snap.DimensionA + snap.DimensionB);
                    return perimeter * snap.LengthByInstanceM;
                }
            }

            if (string.Equals(shape, "Circular", StringComparison.OrdinalIgnoreCase))
            {
                double d = snap.DimensionA > 0 ? snap.DimensionA : snap.DimensionB;
                if (d > 0 && snap.LengthByInstanceM > 0)
                {
                    double perimeter = Math.PI * d;
                    return perimeter * snap.LengthByInstanceM;
                }
            }

            return 0.0;
        }

        private double EstimateFittingArea(ElementSnapshot snap, string shape, out string method)
        {
            double baseArea = EstimateDuctArea(snap, shape);

            if (baseArea > 0)
            {
                method = "GeometricEstimate";
                return baseArea * 1.15;
            }

            method = "FallbackFactor";
            return 0.0;
        }

        private double ResolveMajorDimensionMm(ElementSnapshot snap, string shape)
        {
            double a = snap.DimensionA * 1000.0;
            double b = snap.DimensionB * 1000.0;

            if (string.Equals(shape, "Circular", StringComparison.OrdinalIgnoreCase))
                return Math.Max(a, b);

            return Math.Max(a, b);
        }
    }
}