using System;
using System.Collections.Generic;

namespace NavisBOQ.Core.Steel
{
    public class StructuralSteelCategoryFilterService
    {
        private static readonly HashSet<string> _corrida3Categories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Structural Columns",
                "Pilares estructurales",

                "Structural Framing",
                "Armazon estructural",
                "Armazón estructural",
                "Vigas estructurales",

                "Structural Connections",
                "Conexiones estructurales",

                "Plates",
                "Placas",

                "Structural Stiffeners",
                "Rigidizadores estructurales"
            };

        public bool IsCorrida3Category(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return false;

            return _corrida3Categories.Contains(category.Trim());
        }

        public IReadOnlyCollection<string> GetSupportedCategories()
        {
            return _corrida3Categories;
        }
    }
}