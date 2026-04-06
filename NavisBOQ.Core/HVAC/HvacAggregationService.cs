using System.Collections.Generic;
using System.Linq;

namespace NavisBOQ.Core.HVAC
{
    public class HvacAggregationService
    {
        public List<HvacSummaryRow> Aggregate(List<HvacRunRow> rows)
        {
            return (rows ?? new List<HvacRunRow>())
                .GroupBy(x => new
                {
                    x.Nivel,
                    x.SistemaClasificacion,
                    x.NombreSistema,
                    x.CategoriaBoq,
                    x.Familia,
                    x.Tipo,
                    x.SizeText,
                    x.Material,
                    x.Gauge,
                    x.Unidad
                })
                .Select(g => new HvacSummaryRow
                {
                    Nivel = g.Key.Nivel,
                    SistemaClasificacion = g.Key.SistemaClasificacion,
                    NombreSistema = g.Key.NombreSistema,
                    CategoriaBoq = g.Key.CategoriaBoq,
                    Familia = g.Key.Familia,
                    Tipo = g.Key.Tipo,
                    SizeText = g.Key.SizeText,
                    Material = g.Key.Material,
                    Gauge = g.Key.Gauge,
                    Unidad = g.Key.Unidad,
                    NumElementos = g.Count(),
                    NumTramos = g.Sum(x => x.NumTramos),
                    CantidadTotal = g.Sum(x => x.Cantidad),
                    LongitudTotalMl = g.Sum(x => x.LengthM),
                    AreaTotalM2 = g.Sum(x => x.AreaM2),
                    KgTotal = g.Sum(x => x.Kg)
                })
                .ToList();
        }
    }
}