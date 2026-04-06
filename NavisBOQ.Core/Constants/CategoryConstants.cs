using System;
using System.Collections.Generic;

namespace NavisBOQ.Core.Constants
{
    public static class CategoryConstants
    {
        public static readonly Dictionary<string, string> CanonicalNames =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Walls", "Muros"},
                {"Muros", "Muros"},

                {"Floors", "Losas"},
                {"Suelos", "Losas"},
                {"Losas", "Losas"},

                {"Ceilings", "Plafones"},
                {"Techos", "Plafones"},
                {"Plafones", "Plafones"},

                {"Roofs", "Cubiertas"},
                {"Cubiertas", "Cubiertas"},

                {"Doors", "Puertas"},
                {"Puertas", "Puertas"},

                {"Windows", "Ventanas"},
                {"Ventanas", "Ventanas"},

                {"Structural Framing", "Vigas"},
                {"Vigas estructurales", "Vigas"},

                {"Structural Columns", "Columnas"},
                {"Columnas estructurales", "Columnas"},

                {"Structural Foundations", "Cimentacion"},
                {"Cimentaciones", "Cimentacion"},

                {"Curtain Wall Panels", "Fachada"},
                {"Curtain Panels", "Fachada"},

                {"Ducts", "Ductos"},
                {"Pipes", "Tuberias"},
                {"Cable Trays", "Charolas"},
                {"Conduits", "Conduits"},

                {"Duct Fittings", "Conex Ducto"},
                {"Pipe Fittings", "Conex Tubo"},

                {"Plumbing Fixtures", "Sanitarios"},
                {"Aparatos sanitarios", "Sanitarios"},

                {"Mechanical Equipment", "Eq Mecanico"},
                {"Air Terminals", "Difusores"},
                {"Electrical Equipment", "Tableros"},
                {"Lighting Fixtures", "Luminarias"},

                {"Generic Models", "Generico"},
                {"Modelos genéricos", "Generico"},
                {"Specialty Equipment", "Eq Especial"},
                {"Furniture", "Mobiliario"},
                {"Mobiliario", "Mobiliario"},
                {"Casework", "Carpinteria"}
            };

        public static readonly Dictionary<string, string> DefaultUnits =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Muros", "m2"},
                {"Losas", "m2"},
                {"Plafones", "m2"},
                {"Cubiertas", "m2"},
                {"Puertas", "pza"},
                {"Ventanas", "pza"},
                {"Vigas", "ml"},
                {"Columnas", "ml"},
                {"Cimentacion", "m3"},
                {"Fachada", "m2"},
                {"Ductos", "ml"},
                {"Tuberias", "ml"},
                {"Charolas", "ml"},
                {"Conduits", "ml"},
                {"Conex Ducto", "pza"},
                {"Conex Tubo", "pza"},
                {"Sanitarios", "pza"},
                {"Eq Mecanico", "pza"},
                {"Difusores", "pza"},
                {"Tableros", "pza"},
                {"Luminarias", "pza"},
                {"Generico", "pza"},
                {"Eq Especial", "pza"},
                {"Mobiliario", "pza"},
                {"Carpinteria", "pza"}
            };

        public static readonly HashSet<string> PieceCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Puertas",
                "Ventanas",
                "Sanitarios",
                "Eq Mecanico",
                "Difusores",
                "Tableros",
                "Luminarias",
                "Generico",
                "Eq Especial",
                "Mobiliario",
                "Carpinteria",
                "Conex Ducto",
                "Conex Tubo"
            };

        public static string ResolveCanonicalName(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "";

            string result;
            if (CanonicalNames.TryGetValue(category, out result))
                return result;

            return category.Trim();
        }

        public static string ResolveDefaultUnit(string categoryOrCanonicalName)
        {
            if (string.IsNullOrWhiteSpace(categoryOrCanonicalName))
                return "";

            string canonical = ResolveCanonicalName(categoryOrCanonicalName);

            string result;
            if (DefaultUnits.TryGetValue(canonical, out result))
                return result;

            return "";
        }
    }
}