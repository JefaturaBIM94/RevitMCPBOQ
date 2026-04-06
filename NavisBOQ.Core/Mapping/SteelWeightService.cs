using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Mapping
{
    public class SteelWeightService
    {
        private static readonly string[] SteelKeywords =
        {
            "steel", "acero", "metal", "metalic", "metallic", "w shape", "hss", "pipe",
            "ub-", "uc-", "joist", "angle", "round bar"
        };

        private static readonly string[] ConcreteKeywords =
        {
            "concrete", "concreto", "hormigon", "masonry"
        };

        private static readonly Regex ReNumUnit = new Regex(
            @"(?<num>[-+]?\d+(?:[.,]\d+)?)\s*(?<unit>kg|g|t|ton|tons)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public bool IsSteelCandidate(ElementSnapshot snapshot)
        {
            if (snapshot == null)
                return false;

            string category = snapshot.Category ?? "";

            bool isTargetCategory =
                string.Equals(category, "Structural Framing", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(category, "Structural Columns", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(category, "Vigas estructurales", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(category, "Columnas estructurales", StringComparison.OrdinalIgnoreCase);

            if (!isTargetCategory)
                return false;

            var mat = (snapshot.TypeMaterial ?? snapshot.Material ?? "").Trim();

            if (!string.IsNullOrWhiteSpace(mat))
            {
                if (ConcreteKeywords.Any(k => mat.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

                if (SteelKeywords.Any(k => mat.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
                    return true;
            }

            return true;
        }

        public SteelRow BuildSteelRow(ElementSnapshot snapshot)
        {
            if (snapshot == null)
                return null;

            double pesoKg = 0;
            string metodo = "N/D";

            double effectiveLength = snapshot.CutLengthM > 0 ? snapshot.CutLengthM : snapshot.LengthM;

            double effectiveWeight = 0;
            if (snapshot.NominalWeightKgm > 0)
                effectiveWeight = snapshot.NominalWeightKgm;
            else if (snapshot.LinearWeightKgm > 0)
                effectiveWeight = snapshot.LinearWeightKgm;

            if (effectiveWeight > 0 && effectiveLength > 0)
            {
                pesoKg = effectiveWeight * effectiveLength;
                metodo = "2025+";
            }
            else if (TryReadCustomWeightKg(snapshot, out var customWeightKg) && customWeightKg > 0)
            {
                pesoKg = customWeightKg;
                metodo = "CustomWeight";
            }
            else if (snapshot.VolumeM3 > 0)
            {
                pesoKg = snapshot.VolumeM3 * 7850.0;
                metodo = "Vol×ρ";
            }

            return new SteelRow
            {
                Nivel = snapshot.Level ?? "Sin nivel",
                Categoria = snapshot.Category ?? "",
                Familia = string.IsNullOrWhiteSpace(snapshot.Family) ? "" : snapshot.Family.Trim(),
                Tipo = string.IsNullOrWhiteSpace(snapshot.Type) ? "" : snapshot.Type.Trim(),
                NominalWeight = Math.Round(
                    snapshot.NominalWeightKgm > 0 ? snapshot.NominalWeightKgm : snapshot.LinearWeightKgm,
                    4),
                SectionName = snapshot.SectionName ?? "",
                SectionShape = snapshot.SectionShape ?? "",
                CodeName = snapshot.CodeName ?? "",
                MaterialEst = !string.IsNullOrWhiteSpace(snapshot.TypeMaterial)
                    ? snapshot.TypeMaterial
                    : (snapshot.Material ?? ""),
                Length = Math.Round(effectiveLength, 4),
                Volume = Math.Round(snapshot.VolumeM3, 4),
                PesoKg = Math.Round(pesoKg, 2),
                Metodo = metodo,
                ElemId = snapshot.ElementId ?? "",
                Mark = snapshot.Mark ?? ""
            };
        }

        private static bool TryReadCustomWeightKg(ElementSnapshot snapshot, out double weightKg)
        {
            weightKg = 0;

            var raw = snapshot.CustomWeightRaw ?? "";

            if (string.IsNullOrWhiteSpace(raw))
                return false;

            var m = ReNumUnit.Match(raw.Trim());
            if (!m.Success)
                return false;

            var numRaw = m.Groups["num"].Value.Replace(",", ".");
            double value;
            if (!double.TryParse(numRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return false;

            var unit = (m.Groups["unit"].Value ?? "").Trim().ToLowerInvariant();

            switch (unit)
            {
                case "":
                case "kg":
                    weightKg = value;
                    return true;
                case "g":
                    weightKg = value / 1000.0;
                    return true;
                case "t":
                case "ton":
                case "tons":
                    weightKg = value * 1000.0;
                    return true;
                default:
                    return false;
            }
        }
    }
}
