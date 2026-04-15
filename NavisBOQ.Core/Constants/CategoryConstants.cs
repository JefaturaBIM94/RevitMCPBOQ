using System;
using System.Collections.Generic;

namespace NavisBOQ.Core.Constants
{
    public static class CategoryConstants
    {
        /// <summary>
        /// Mapeo de categorías Revit/Navis (EN/ES) a nombre BOQ canónico y unidad por defecto.
        /// </summary>
        public static readonly Dictionary<string, (string Name, string Unit)> Mapa =
            new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase)
            {
                // Arquitectura / General
                {"Walls", ("Muros", "m2")},
                {"Muros", ("Muros", "m2")},

                {"Floors", ("Losas", "m2")},
                {"Suelos", ("Losas", "m2")},
                {"Losas", ("Losas", "m2")},

                {"Roofs", ("Cubiertas", "m2")},
                {"Cubiertas", ("Cubiertas", "m2")},

                {"Ceilings", ("Plafones", "m2")},
                {"Techos", ("Plafones", "m2")},
                {"Plafones", ("Plafones", "m2")},

                {"Doors", ("Puertas", "pza")},
                {"Puertas", ("Puertas", "pza")},

                {"Windows", ("Ventanas", "pza")},
                {"Ventanas", ("Ventanas", "pza")},

                // Muro cortina — FIX idioma ES
                {"Curtain Wall Panels", ("Paneles muro cortina", "m2")},
                {"Curtain Panels", ("Paneles muro cortina", "m2")},
                {"Paneles de muro cortina", ("Paneles muro cortina", "m2")},
                {"Paneles muro cortina", ("Paneles muro cortina", "m2")},

                // Barandales
                {"Railings", ("Barandales", "ml")},
                {"Barandillas", ("Barandales", "ml")},
                {"Barandales", ("Barandales", "ml")},

                // Sanitarios — FIX idioma ES
                {"Plumbing Fixtures", ("Aparatos sanitarios", "pza")},
                {"Aparatos sanitarios", ("Aparatos sanitarios", "pza")},

                // Equipos especiales — FIX idioma ES
                {"Specialty Equipment", ("Equipos especializados", "pza")},
                {"Equipos especializados", ("Equipos especializados", "pza")},

                // Modelos genéricos
                {"Generic Models", ("Modelos genéricos", "pza")},
                {"Modelos genéricos", ("Modelos genéricos", "pza")},

                // Estructura
                {"Structural Framing", ("Vigas", "ml")},
                {"Vigas estructurales", ("Vigas", "ml")},

                {"Structural Columns", ("Columnas", "ml")},
                {"Columnas estructurales", ("Columnas", "ml")},

                {"Structural Foundations", ("Cimentacion", "m3")},
                {"Cimentaciones", ("Cimentacion", "m3")},

                // MEP / Eléctrica / HVAC
                {"Ducts", ("Ductos", "ml")},
                {"Duct Fittings", ("Conex Ducto", "pza")},
                {"Pipes", ("Tuberias", "ml")},
                {"Pipe Fittings", ("Conex Tubo", "pza")},
                {"Electrical Equipment", ("Equipos eléctricos", "pza")},
                {"Equipos eléctricos", ("Equipos eléctricos", "pza")},
                {"Lighting Fixtures", ("Luminarias", "pza")},
                {"Luminarias", ("Luminarias", "pza")},
                {"Cable Trays", ("Charolas", "ml")},
                {"Charolas", ("Charolas", "ml")},
                {"Conduits", ("Conduits", "ml")},
                {"Conductos", ("Conduits", "ml")},
                {"Mechanical Equipment", ("Equipos mecánicos", "pza")},
                {"Equipos mecánicos", ("Equipos mecánicos", "pza")},
                {"Air Terminals", ("Difusores", "pza")},
                {"Difusores", ("Difusores", "pza")},
                {"Electrical Fixtures", ("Aparatos eléctricos", "pza")},
                {"Aparatos eléctricos", ("Aparatos eléctricos", "pza")}
            };

        /// <summary>
        /// Categorías que deben cuantificarse por pieza.
        /// IMPORTANTE: Railings/Barandales NO van aquí porque se cuantifican por longitud.
        /// </summary>
        public static readonly HashSet<string> PieceCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Doors", "Puertas",
                "Windows", "Ventanas",
                "Plumbing Fixtures", "Aparatos sanitarios",
                "Specialty Equipment", "Equipos especializados",
                "Generic Models", "Modelos genéricos",
                "Duct Fittings", "Conex Ducto",
                "Pipe Fittings", "Conex Tubo",
                "Electrical Equipment", "Equipos eléctricos",
                "Lighting Fixtures", "Luminarias",
                "Mechanical Equipment", "Equipos mecánicos",
                "Air Terminals", "Difusores",
                "Electrical Fixtures", "Aparatos eléctricos"
            };

        public static bool TryMap(string category, out string canonicalName, out string defaultUnit)
        {
            canonicalName = "";
            defaultUnit = "";

            if (string.IsNullOrWhiteSpace(category))
                return false;

            if (Mapa.TryGetValue(category.Trim(), out var value))
            {
                canonicalName = value.Name;
                defaultUnit = value.Unit;
                return true;
            }

            return false;
        }

        public static string ResolveCanonicalName(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "";

            if (Mapa.TryGetValue(category.Trim(), out var value))
                return value.Name;

            return category.Trim();
        }

        public static string ResolveDefaultUnit(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "pza";

            if (Mapa.TryGetValue(category.Trim(), out var value))
                return value.Unit;

            return PieceCategories.Contains(category.Trim()) ? "pza" : "pza";
        }

        public static bool IsPieceCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return false;

            return PieceCategories.Contains(category.Trim());
        }
    }
}