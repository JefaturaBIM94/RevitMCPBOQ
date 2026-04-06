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

        // IMPORTANTE:
        // En esta implementación:
        // - Pipes / Tubos = tubos eléctricos
        // - Pipe Fittings / Uniones de tubo = accesorios eléctricos
        // - Se excluyen explícitamente categorías de piping/plumbing/mecánicas
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
                            "Aparatos eléctricos",
                            "Uniones de tubo",
                            "Uniones de bandeja de cables",
                            "Modelos genéricos",

                            // EN
                            "Electrical Fixtures",
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

                            // EN
                            "Lighting Devices"
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
                            "Tubos",
                            "Tuberías flexibles",
                            "Bandejas de cables",

                            // EN
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
                            "Tubos",
                            "Tuberías flexibles",

                            // EN
                            "Pipes",
                            "Flexible Pipes"
                        }
                    }
                }
            };
    }
}