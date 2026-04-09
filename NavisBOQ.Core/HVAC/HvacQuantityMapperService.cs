using NavisBOQ.Core.Constants;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.HVAC
{
    public class HvacQuantityMapperService
    {
        private readonly HvacSystemResolverService _systemResolver;
        private readonly HvacSheetMetalWeightService _weightService;

        public HvacQuantityMapperService()
        {
            _systemResolver = new HvacSystemResolverService();
            _weightService = new HvacSheetMetalWeightService();
        }

        public HvacRunRow Map(ElementSnapshot snap, string boqCategory, string unit)
        {
            var system = _systemResolver.Resolve(snap);

            var row = new HvacRunRow
            {
                Nivel = CleanOrDefault(snap.Level, "Sin nivel"),
                SistemaClasificacion = CleanOrDefault(system.Classification, "Sin sistema HVAC"),
                NombreSistema = CleanOrDefault(system.Name, "Sin sistema HVAC"),
                TipoSistema = Clean(system.Type),

                CategoriaBoq = ResolveBoqCategory(snap, boqCategory),
                CategoriaRevit = Clean(snap.Category),
                Familia = Clean(snap.Family),
                Tipo = Clean(snap.Type),
                ElemId = Clean(snap.ElementId),

                FittingSubcategory = ResolveFittingSubcategory(snap),

                Material = ResolveMaterial(snap),
                SegmentoTuberia = ResolvePipeSegment(snap),
                SizeText = ResolveSizeText(snap),
                Shape = ResolveShapeText(snap),

                WidthM = ResolveWidth(snap),
                HeightM = ResolveHeight(snap),
                DiameterM = ResolveDiameter(snap),
                LengthM = ResolveLength(snap),
                AreaM2 = snap.AreaM2 > 0 ? snap.AreaM2 : 0.0,

                Unidad = unit
            };

            if (HvacCategoryConstants.IsDuctLike(snap.Category))
            {
                row.Cantidad = row.LengthM;
                row.NumTramos = row.LengthM > 0 ? 1 : 0;
                row.Unidad = "ml";

                row = _weightService.Enrich(snap, row);
                row.SizeText = NormalizeSizeText(row, snap);
                return row;
            }

            if (HvacCategoryConstants.IsDuctFittingLike(snap.Category))
            {
                row.Cantidad = 1;
                row.NumTramos = 0;
                row.Unidad = "pza";

                row = _weightService.Enrich(snap, row);
                row.SizeText = NormalizeSizeText(row, snap);
                return row;
            }

            if (HvacCategoryConstants.IsPipeLike(snap.Category))
            {
                row.Cantidad = row.LengthM;
                row.NumTramos = row.LengthM > 0 ? 1 : 0;
                row.Unidad = "ml";
                row.SizeText = NormalizeSizeText(row, snap);
                return row;
            }

            if (HvacCategoryConstants.IsPipeFittingLike(snap.Category))
            {
                row.Cantidad = 1;
                row.NumTramos = 0;
                row.Unidad = "pza";
                row.SizeText = NormalizeSizeText(row, snap);
                return row;
            }

            row.Cantidad = 1;
            row.NumTramos = 0;
            row.Unidad = "pza";
            row.SizeText = NormalizeSizeText(row, snap);

            return row;
        }

        private string ResolveBoqCategory(ElementSnapshot snap, string fallbackBoqCategory)
        {
            if (snap == null)
                return Clean(fallbackBoqCategory);

            if (HvacCategoryConstants.IsDuctFittingLike(snap.Category))
            {
                string sub = ResolveFittingSubcategory(snap);

                if (sub == "Transition")
                    return "Reducciones";

                if (sub == "Elbow" || sub == "Tee" || sub == "Tap" || sub == "Cross" || sub == "Offset")
                    return "Codos y accesorios";

                return "Uniones de conducto";
            }

            return Clean(fallbackBoqCategory);
        }

        private string ResolveFittingSubcategory(ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(snap.FittingSubcategory))
                return Clean(snap.FittingSubcategory);

            string text = ((snap.Type ?? "") + " " + (snap.Family ?? "")).ToLowerInvariant();

            if (text.Contains("transition") || text.Contains("reduccion") || text.Contains("reduction"))
                return "Transition";

            if (text.Contains("elbow") || text.Contains("codo"))
                return "Elbow";

            if (text.Contains("tee") || text.Contains("yee") || text.Contains("wye"))
                return "Tee";

            if (text.Contains("tap") || text.Contains("takeoff") || text.Contains("take off"))
                return "Tap";

            if (text.Contains("offset"))
                return "Offset";

            if (text.Contains("union") || text.Contains("coupling") || text.Contains("connector"))
                return "GenericFitting";

            if (text.Contains("cross"))
                return "Cross";

            return "GenericFitting";
        }

        private string ResolveMaterial(ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(snap.TypeMaterial))
                return Clean(snap.TypeMaterial);

            if (!string.IsNullOrWhiteSpace(snap.Material))
                return Clean(snap.Material);

            if (!string.IsNullOrWhiteSpace(snap.TypeComments))
                return Clean(snap.TypeComments);

            return "";
        }

        private string ResolvePipeSegment(ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(snap.SegmentName))
                return Clean(snap.SegmentName);

            if (!string.IsNullOrWhiteSpace(snap.SegmentDescription))
                return Clean(snap.SegmentDescription);

            if (!string.IsNullOrWhiteSpace(snap.TypeComments))
                return Clean(snap.TypeComments);

            return "";
        }

        private string ResolveSizeText(ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(snap.SizeText))
                return Clean(snap.SizeText);

            if (!string.IsNullOrWhiteSpace(snap.OverallSizeText))
                return Clean(snap.OverallSizeText);

            if (!string.IsNullOrWhiteSpace(snap.FreeSizeText))
                return Clean(snap.FreeSizeText);

            return "";
        }

        private string ResolveShapeText(ElementSnapshot snap)
        {
            if (snap.DiameterM > 0 || snap.EquivalentDiameterM > 0)
                return "Circular";

            if ((snap.DimensionA > 0 && snap.DimensionB > 0) ||
                (snap.DuctWidthM > 0 && snap.DuctHeightM > 0) ||
                (snap.DuctWidth1M > 0 && snap.DuctHeight1M > 0))
                return "Rectangular";

            if (!string.IsNullOrWhiteSpace(snap.SizeText))
            {
                if (snap.SizeText.Contains("x"))
                    return "Rectangular";

                return "Circular";
            }

            return "";
        }

        private double ResolveWidth(ElementSnapshot snap)
        {
            if (snap.DimensionA > 0) return snap.DimensionA;
            if (snap.DuctWidthM > 0) return snap.DuctWidthM;
            if (snap.DuctWidth1M > 0) return snap.DuctWidth1M;
            if (snap.DuctWidth2M > 0) return snap.DuctWidth2M;
            return 0.0;
        }

        private double ResolveHeight(ElementSnapshot snap)
        {
            if (snap.DimensionB > 0) return snap.DimensionB;
            if (snap.DuctHeightM > 0) return snap.DuctHeightM;
            if (snap.DuctHeight1M > 0) return snap.DuctHeight1M;
            if (snap.DuctHeight2M > 0) return snap.DuctHeight2M;
            return 0.0;
        }

        private double ResolveDiameter(ElementSnapshot snap)
        {
            if (snap.DiameterM > 0) return snap.DiameterM;
            if (snap.EquivalentDiameterM > 0) return snap.EquivalentDiameterM;
            return 0.0;
        }

        private double ResolveLength(ElementSnapshot snap)
        {
            if (snap.LengthByInstanceM > 0) return snap.LengthByInstanceM;
            if (snap.LengthM > 0) return snap.LengthM;
            if (snap.DuctLengthM > 0) return snap.DuctLengthM;
            if (snap.DuctLength1M > 0) return snap.DuctLength1M;
            return 0.0;
        }

        private string NormalizeSizeText(HvacRunRow row, ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(row.SizeText))
                return row.SizeText;

            if (row.Shape == "Rectangular" && row.WidthM > 0 && row.HeightM > 0)
                return $"{ToMmText(row.WidthM)}x{ToMmText(row.HeightM)}";

            if (row.Shape == "Circular" && row.DiameterM > 0)
                return $"Ø{ToMmText(row.DiameterM)}";

            return ResolveSizeText(snap);
        }

        private string ToMmText(double meters)
        {
            var mm = meters * 1000.0;
            return ((int)System.Math.Round(mm)).ToString();
        }

        private string CleanOrDefault(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private string Clean(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
        }
    }
}