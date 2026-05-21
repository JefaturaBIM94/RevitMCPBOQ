using System;
using System.Collections.Generic;

namespace NavisBOQ.Core.Constants
{
    public static class ElectricalCategoryConstants
    {
        public class ElectricalCategoryRule
        {
            public string CanonicalName { get; set; }
            public string Unit { get; set; }
            public List<string> RevitCategories { get; set; }

            public ElectricalCategoryRule()
            {
                CanonicalName = "";
                Unit = "";
                RevitCategories = new List<string>();
            }
        }

        public static readonly HashSet<string> StrictlyForbiddenPlumbingCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Piping",
                "Piping Systems",
                "Piping System",
                "Plumbing",
                "Plumbing Fixtures",
                "Plumbing Equipment",
                "Sanitary",
                "Sanitary Piping",
                "Domestic Cold Water",
                "Domestic Hot Water",
                "Hydronic Supply",
                "Hydronic Return",
                "Fire Protection"
            };

        public static readonly HashSet<string> LinearRevitCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Conduits",
                "Conduit",
                "Conductos",
                "Conducto",
                "Tubos",
                "Tubo",
                "Pipes",
                "Pipe",
                "Cable Trays",
                "Charolas"
            };

        public static readonly HashSet<string> SizeDrivenRevitCategories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Conduits",
                "Conduit",
                "Conductos",
                "Conducto",
                "Tubos",
                "Tubo",
                "Conduit Fittings",
                "Conduit Fitting",
                "Accesorios de conducto",
                "Accesorios de conduits"
            };

        public static readonly Dictionary<string, ElectricalCategoryRule> BoqMap =
            new Dictionary<string, ElectricalCategoryRule>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "Conexiones y accesorios",
                    new ElectricalCategoryRule
                    {
                        CanonicalName = "Conexiones y accesorios",
                        Unit = "pza",
                        RevitCategories = new List<string>
                        {
                            // ES
                            "Accesorios de tuberías",
                            "Accesorios de conducto",
                            "Accesorios de conduits",
                            "Aparatos eléctricos",
                            "Dispositivos de comunicación",
                            "Uniones de tubo",
                            "Uniones de bandeja de cables",
                            "Modelos genéricos",

                            // EN
                            "Conduit Fittings",
                            "Electrical Fixtures",
                            "Communication Devices",
                            "Pipe Fittings",
                            "Cable Tray Fittings",
                            "Generic Models"
                        }
                    }
                },
                {
                    "Tableros",
                    new ElectricalCategoryRule
                    {
                        CanonicalName = "Tableros",
                        Unit = "pza",
                        RevitCategories = new List<string>
                        {
                            // ES
                            "Equipos eléctricos",

                            // EN
                            "Electrical Equipment"
                        }
                    }
                },
                {
                    "Luminarias",
                    new ElectricalCategoryRule
                    {
                        CanonicalName = "Luminarias",
                        Unit = "pza",
                        RevitCategories = new List<string>
                        {
                            // ES
                            "Dispositivos de iluminación",
                            "Luminarias",

                            // EN
                            "Lighting Devices",
                            "Lighting Fixtures"
                        }
                    }
                },
                {
                    "Tuberias",
                    new ElectricalCategoryRule
                    {
                        CanonicalName = "Tuberias",
                        Unit = "ml",
                        RevitCategories = new List<string>
                        {
                            // ES
                            "Conductos",
                            "Tubos",
                            "Tuberías flexibles",
                            "Bandejas de cables",

                            // EN
                            "Conduits",
                            "Conduit",
                            "Pipes",
                            "Flexible Pipes",
                            "Cable Trays"
                        }
                    }
                },
                {
                    "Tierras y pararrayos",
                    new ElectricalCategoryRule
                    {
                        CanonicalName = "Tierras y pararrayos",
                        Unit = "pza",
                        RevitCategories = new List<string>
                        {
                            // ES
                            "Sistemas de puesta a tierra",
                            "Pararrayos",

                            // EN
                            "Grounding",
                            "Lightning Protection"
                        }
                    }
                }
            };

        public static bool IsLinearCategory(string revitCategory)
        {
            if (string.IsNullOrWhiteSpace(revitCategory))
                return false;

            string category = revitCategory.Trim();
            return LinearRevitCategories.Contains(category);
        }

        public static bool IsSizeDrivenCategory(string revitCategory)
        {
            if (string.IsNullOrWhiteSpace(revitCategory))
                return false;

            string category = revitCategory.Trim();
            return SizeDrivenRevitCategories.Contains(category);
        }
    }
}
