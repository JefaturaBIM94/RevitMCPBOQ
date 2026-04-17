using System.Collections.Generic;
using System.Linq;

namespace NavisBOQ.Core.FPS
{
    public class FpsAggregationService
    {
        public List<FpsSummaryRow> Aggregate(List<FpsRunRow> rows)
        {
            return (rows ?? new List<FpsRunRow>())
                .GroupBy(x => new
                {
                    x.Nivel,
                    x.SistemaClasificacion,
                    x.NombreSistema,
                    x.CategoriaBoq,
                    x.Familia,
                    x.Tipo,
                    x.Material,
                    x.SizeText,
                    x.Unidad
                })
                .Select(g => new FpsSummaryRow
                {
                    Nivel = g.Key.Nivel,
                    SistemaClasificacion = g.Key.SistemaClasificacion,
                    NombreSistema = g.Key.NombreSistema,
                    CategoriaBoq = g.Key.CategoriaBoq,
                    Familia = g.Key.Familia,
                    Tipo = g.Key.Tipo,
                    Material = g.Key.Material,
                    SizeText = g.Key.SizeText,
                    Unidad = g.Key.Unidad,
                    NumElementos = g.Count(),
                    NumTramos = g.Sum(x => x.NumTramos),
                    CantidadTotal = System.Math.Round(g.Sum(x => x.Cantidad), 3),
                    LongitudTotalMl = System.Math.Round(g.Sum(x => x.LengthM), 3)
                })
                .OrderBy(x => x.Nivel)
                .ThenBy(x => x.SistemaClasificacion)
                .ThenBy(x => x.NombreSistema)
                .ThenBy(x => x.CategoriaBoq)
                .ThenBy(x => x.Familia)
                .ThenBy(x => x.Tipo)
                .ToList();
        }
    }
}