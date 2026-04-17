using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.FPS
{
    public class FpsQuantityMapperService
    {
        private readonly FpsSystemResolverService _systemResolver;

        public FpsQuantityMapperService()
        {
            _systemResolver = new FpsSystemResolverService();
        }

        public FpsRunRow Map(ElementSnapshot snap, string boqCategory, string unit)
        {
            var system = _systemResolver.Resolve(snap);

            var row = new FpsRunRow
            {
                Nivel = CleanOrDefault(snap.Level, "Sin nivel"),
                SistemaClasificacion = CleanOrDefault(system.Classification, "Sin sistema FPS"),
                NombreSistema = CleanOrDefault(system.Name, "Sin sistema FPS"),
                TipoSistema = Clean(system.Type),

                CategoriaBoq = Clean(boqCategory),
                CategoriaRevit = Clean(snap.Category),
                Familia = Clean(snap.Family),
                Tipo = Clean(snap.Type),
                ElemId = Clean(snap.ElementId),

                Material = ResolveMaterial(snap),
                SegmentoTuberia = ResolvePipeSegment(snap),
                SizeText = ResolveSizeText(snap),
                DiameterM = ResolveDiameter(snap),
                LengthM = ResolveLength(snap),
                Unidad = unit
            };

            if (FpsCategoryConstants.IsPipeLike(snap.Category) || FpsCategoryConstants.IsFlexPipeLike(snap.Category))
            {
                row.Cantidad = row.LengthM;
                row.NumTramos = row.LengthM > 0 ? 1 : 0;
                row.Unidad = "ml";
                return row;
            }

            row.Cantidad = 1;
            row.NumTramos = 0;
            row.Unidad = "pza";
            return row;
        }

        private string ResolveMaterial(ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(snap.TypeMaterial))
                return Clean(snap.TypeMaterial);

            if (!string.IsNullOrWhiteSpace(snap.Material))
                return Clean(snap.Material);

            return "";
        }

        private string ResolvePipeSegment(ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(snap.SegmentName))
                return Clean(snap.SegmentName);

            if (!string.IsNullOrWhiteSpace(snap.SegmentDescription))
                return Clean(snap.SegmentDescription);

            return "";
        }

        private string ResolveSizeText(ElementSnapshot snap)
        {
            if (!string.IsNullOrWhiteSpace(snap.SizeText))
                return Clean(snap.SizeText);

            if (!string.IsNullOrWhiteSpace(snap.OverallSizeText))
                return Clean(snap.OverallSizeText);

            return "";
        }

        private double ResolveDiameter(ElementSnapshot snap)
        {
            if (snap.OutsideDiameterM > 0)
                return snap.OutsideDiameterM;

            if (snap.DiameterM > 0)
                return snap.DiameterM;

            if (snap.EquivalentDiameterM > 0)
                return snap.EquivalentDiameterM;

            return 0.0;
        }

        private double ResolveLength(ElementSnapshot snap)
        {
            if (snap.LengthByInstanceM > 0)
                return snap.LengthByInstanceM;

            if (snap.LengthM > 0)
                return snap.LengthM;

            return 0.0;
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