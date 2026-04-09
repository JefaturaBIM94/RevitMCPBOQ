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
                    x.FittingSubcategory,
                    x.Familia,
                    x.Tipo,
                    x.SizeText,
                    x.Material,
                    x.Gauge,
                    x.GaugeCode,
                    x.Unidad
                })
                .Select(g => new HvacSummaryRow
                {
                    Nivel = g.Key.Nivel,
                    SistemaClasificacion = g.Key.SistemaClasificacion,
                    NombreSistema = g.Key.NombreSistema,
                    CategoriaBoq = g.Key.CategoriaBoq,
                    FittingSubcategory = g.Key.FittingSubcategory,
                    Familia = g.Key.Familia,
                    Tipo = g.Key.Tipo,
                    SizeText = g.Key.SizeText,
                    Material = g.Key.Material,
                    Gauge = g.Key.Gauge,
                    GaugeCode = g.Key.GaugeCode,
                    Unidad = g.Key.Unidad,
                    NumElementos = g.Count(),
                    NumTramos = g.Sum(x => x.NumTramos),
                    CantidadTotal = Round(g.Sum(x => x.Cantidad), 3),
                    LongitudTotalMl = Round(g.Sum(x => x.LengthM), 3),
                    AreaTotalM2 = Round(g.Sum(x => x.AreaM2), 3),
                    KgTotal = Round(g.Sum(x => x.Kg), 3)
                })
                .OrderBy(x => x.Nivel)
                .ThenBy(x => x.SistemaClasificacion)
                .ThenBy(x => x.NombreSistema)
                .ThenBy(x => x.CategoriaBoq)
                .ThenBy(x => x.FittingSubcategory)
                .ThenBy(x => x.GaugeCode)
                .ThenBy(x => x.Familia)
                .ThenBy(x => x.Tipo)
                .ThenBy(x => x.SizeText)
                .ToList();
        }

        private double Round(double value, int digits)
        {
            return System.Math.Round(value, digits);
        }
    }
}