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
                Nivel = snap.Level ?? "Sin nivel",
                SistemaClasificacion = system.Classification,
                NombreSistema = system.Name,
                TipoSistema = system.Type,

                CategoriaBoq = boqCategory,
                CategoriaRevit = snap.Category ?? "",
                Familia = snap.Family ?? "",
                Tipo = snap.Type ?? "",
                ElemId = snap.ElementId ?? "",

                Material = !string.IsNullOrWhiteSpace(snap.TypeMaterial) ? snap.TypeMaterial : snap.Material,
                SegmentoTuberia = snap.TypeComments ?? "",
                SizeText = snap.SizeText ?? "",
                WidthM = snap.DimensionA,
                HeightM = snap.DimensionB,
                DiameterM = 0.0,
                LengthM = snap.LengthByInstanceM > 0 ? snap.LengthByInstanceM : snap.LengthM,
                AreaM2 = snap.AreaM2,
                Unidad = unit
            };

            if (HvacCategoryConstants.IsDuctLike(snap.Category))
            {
                row.Cantidad = row.LengthM;
                row.NumTramos = 1;
                row.Unidad = "ml";
                return _weightService.Enrich(snap, row);
            }

            if (HvacCategoryConstants.IsDuctFittingLike(snap.Category))
            {
                row.Cantidad = 1;
                row.NumTramos = 0;
                row.Unidad = "pza";
                return _weightService.Enrich(snap, row);
            }

            if (HvacCategoryConstants.IsPipeLike(snap.Category))
            {
                row.Cantidad = row.LengthM;
                row.NumTramos = 1;
                row.Unidad = "ml";
                return row;
            }

            if (HvacCategoryConstants.IsPipeFittingLike(snap.Category))
            {
                row.Cantidad = 1;
                row.NumTramos = 0;
                row.Unidad = "pza";
                return row;
            }

            row.Cantidad = 1;
            row.NumTramos = 0;
            row.Unidad = "pza";
            return row;
        }
    }
}
