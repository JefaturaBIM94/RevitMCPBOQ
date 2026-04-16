using System;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Steel
{
    public class StructuralSteelWeightService
    {
        private const double SteelDensityKgm3 = 7850.0;

        public double ResolveWeightKg(
            double nominalWeightKgm,
            double linearWeightKgm,
            double linealWeightKgm,
            double lengthM,
            double weight2022Kg,
            double volumeM3,
            out string metodo,
            out string advertencia)
        {
            metodo = "N/D";
            advertencia = "";

            if (nominalWeightKgm > 0 && lengthM > 0)
            {
                metodo = "NominalWeight×Length";
                return nominalWeightKgm * lengthM;
            }

            if (linearWeightKgm > 0 && lengthM > 0)
            {
                metodo = "LinearWeight×Length";
                return linearWeightKgm * lengthM;
            }

            if (linealWeightKgm > 0 && lengthM > 0)
            {
                metodo = "LinealWeight×Length";
                return linealWeightKgm * lengthM;
            }

            if (weight2022Kg > 0)
            {
                metodo = "Weight(2022)";
                return weight2022Kg;
            }

            if (volumeM3 > 0)
            {
                metodo = "Vol×ρ";
                advertencia = "Peso obtenido por fallback Volume × 7850 kg/m3.";
                return volumeM3 * 7850.0; 
            }

            advertencia = "No se encontraron parámetros suficientes para calcular el peso.";
            return 0.0;
        }

        public bool IsConcreteLike(string material)
        {
            if (string.IsNullOrWhiteSpace(material))
                return false;

            string value = material.Trim();

            return value.IndexOf("concrete", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("concreto", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("hormigon", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("hormigón", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("masonry", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public bool IsSteelLike(string material)
        {
            if (string.IsNullOrWhiteSpace(material))
                return true;

            string value = material.Trim();

            return value.IndexOf("steel", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("acero", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("metal", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("metalic", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("metallic", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("stiffener", StringComparison.OrdinalIgnoreCase) >= 0
                || value.IndexOf("plate", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}