using System;
using System.Collections.Generic;

namespace NavisBOQ.Core.Constants
{
    public static class HvacCategoryConstants
    {
        public static readonly Dictionary<string, (string BoqCategory, string Unit)> Map =
            new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
            {
                { "Ducts", ("Conductos", "ml") },
                { "Conductos", ("Conductos", "ml") },

                { "Duct Fittings", ("Uniones de conducto", "pza") },
                { "Uniones de conducto", ("Uniones de conducto", "pza") },

                { "Duct Accessories", ("Accesorios de conductos", "pza") },
                { "Accesorios de conductos", ("Accesorios de conductos", "pza") },

                { "Flex Ducts", ("Conductos flexibles", "ml") },
                { "Conductos flexibles", ("Conductos flexibles", "ml") },

                { "Pipes", ("Tuberias", "ml") },
                { "Tuberías", ("Tuberias", "ml") },

                { "Pipe Fittings", ("Uniones de tuberia", "pza") },
                { "Uniones de tubería", ("Uniones de tuberia", "pza") },

                { "Pipe Accessories", ("Accesorios de tuberia", "pza") },
                { "Accesorios de tuberías", ("Accesorios de tuberia", "pza") },

                { "Mechanical Equipment", ("Equipos mecanicos", "pza") },
                { "Equipos mecánicos", ("Equipos mecanicos", "pza") },

                { "Air Terminals", ("Terminales de aire", "pza") },
                { "Terminales de aire", ("Terminales de aire", "pza") },

                { "Generic Models", ("Modelos genericos HVAC", "pza") },
                { "Modelos genéricos", ("Modelos genericos HVAC", "pza") }
            };

        public static bool IsDuctLike(string category)
        {
            return Eq(category, "Ducts")
                || Eq(category, "Conductos")
                || Eq(category, "Flex Ducts")
                || Eq(category, "Conductos flexibles");
        }

        public static bool IsDuctFittingLike(string category)
        {
            return Eq(category, "Duct Fittings")
                || Eq(category, "Uniones de conducto");
        }

        public static bool IsPipeLike(string category)
        {
            return Eq(category, "Pipes")
                || Eq(category, "Tuberías");
        }

        public static bool IsPipeFittingLike(string category)
        {
            return Eq(category, "Pipe Fittings")
                || Eq(category, "Uniones de tubería");
        }

        public static bool IsPieceLike(string category)
        {
            return Eq(category, "Duct Accessories")
                || Eq(category, "Accesorios de conductos")
                || Eq(category, "Pipe Accessories")
                || Eq(category, "Accesorios de tuberías")
                || Eq(category, "Mechanical Equipment")
                || Eq(category, "Equipos mecánicos")
                || Eq(category, "Air Terminals")
                || Eq(category, "Terminales de aire")
                || Eq(category, "Generic Models")
                || Eq(category, "Modelos genéricos");
        }

        private static bool Eq(string a, string b)
        {
            return string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}