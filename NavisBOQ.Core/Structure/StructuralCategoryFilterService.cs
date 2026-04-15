using System;
using System.Collections.Generic;

namespace NavisBOQ.Core.Structure
{
    public class StructuralCategoryFilterService
    {
        private static readonly HashSet<string> _corrida2Categories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Walls",
                "Muros",

                "Floors",
                "Suelos",
                "Losas",

                "Roofs",
                "Cubiertas",

                "Structural Columns",
                "Pilares estructurales",

                "Structural Framing",
                "Armazón estructural",

                "Structural Foundations",
                "Cimentación estructural",
                "Jácena",


                "Pads",
                "Pedestales",
                "Zapatas"
            };

        public bool IsCorrida2Category(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return false;

            return _corrida2Categories.Contains(category.Trim());
        }

        public IReadOnlyCollection<string> GetSupportedCategories()
        {
            return _corrida2Categories;
        }
    }
}