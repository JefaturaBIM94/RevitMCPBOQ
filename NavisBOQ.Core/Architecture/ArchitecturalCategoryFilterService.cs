using System;
using System.Collections.Generic;

namespace NavisBOQ.Core.Architecture
{
    public class ArchitecturalCategoryFilterService
    {
        private static readonly HashSet<string> _corrida1Categories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Walls",
                "Muros",

                "Floors",
                "Suelos",
                "Losas",

                "Roofs",
                "Cubiertas",

                "Ceilings",
                "Techos",
                "Plafones",

                "Doors",
                "Puertas",

                "Windows",
                "Ventanas",

                "Curtain Wall Panels",
                "Curtain Panels",
                "Paneles de muro cortina",
                "Paneles muro cortina",

                "Railings",
                "Barandillas",
                "Barandales",

                "Plumbing Fixtures",
                "Aparatos sanitarios",

                "Specialty Equipment",
                "Equipos especializados",

                "Generic Models",
                "Modelos genéricos"
            };

        public bool IsCorrida1Category(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return false;

            return _corrida1Categories.Contains(category.Trim());
        }

        public IReadOnlyCollection<string> GetSupportedCategories()
        {
            return _corrida1Categories;
        }
    }
}