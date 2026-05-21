using System.Collections.Generic;
using System.Linq;

namespace NavisBOQ.Core.Electrical
{
    public class ElectricalAggregationService
    {
        public List<ElectricalSummaryRow> Aggregate(List<ElectricalRunRow> rows)
        {
            return (rows ?? new List<ElectricalRunRow>())
                .GroupBy(x => new
                {
                    x.Nivel,
                    x.Sistema,
                    x.CategoriaBoq,
                    x.CategoriaRevit,
                    x.Familia,
                    x.Tipo,
                    x.SizeText,
                    x.Unidad
                })
                .Select(g => new ElectricalSummaryRow
                {
                    Nivel = g.Key.Nivel,
                    Sistema = g.Key.Sistema,
                    CategoriaBoq = g.Key.CategoriaBoq,
                    CategoriaRevit = g.Key.CategoriaRevit,
                    Familia = g.Key.Familia,
                    Tipo = g.Key.Tipo,
                    SizeText = g.Key.SizeText,
                    Unidad = g.Key.Unidad,
                    NumElementos = g.Count(),
                    CantidadTotal = g.Sum(x => x.Cantidad),
                    LongitudTotalMl = g.Sum(x => x.LongitudTotalMl),
                    NumTramos = g.Sum(x => x.NumTramos)
                })
                .OrderBy(x => x.Nivel)
                .ThenBy(x => x.Sistema)
                .ThenBy(x => x.CategoriaBoq)
                .ThenBy(x => x.CategoriaRevit)
                .ThenBy(x => x.Familia)
                .ThenBy(x => x.Tipo)
                .ThenBy(x => x.SizeText)
                .ToList();
        }
    }
}
