using System;
using System.Collections.Generic;
using System.Linq;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Rebar
{
    public class RebarAggregationService
    {
        public List<object> Aggregate(List<RebarRunRow> rows)
        {
            return rows
                .GroupBy(r => new
                {
                    r.Nivel,
                    r.Categoria,
                    r.Tipo,
                    r.BarNumber,
                    r.DiameterMm,
                    r.LinearWeightKgm
                })
                .Select(g => (object)new
                {
                    Nivel = g.Key.Nivel,
                    Categoria = g.Key.Categoria,
                    Tipo = g.Key.Tipo,
                    NumeroVarilla = g.Key.BarNumber,
                    DiametroMm = Math.Round(g.Key.DiameterMm, 3),
                    PesoLinealKgm = Math.Round(g.Key.LinearWeightKgm, 3),

                    Instancias = g.Count(),
                    CantidadBarras = g.Sum(x => x.Quantity),

                    LongitudBarraM = Math.Round(g.Average(x => x.BarLengthM), 3),
                    LongitudTotalM = Math.Round(g.Sum(x => x.TotalLengthM), 3),
                    PesoTotalKg = Math.Round(g.Sum(x => x.TotalWeightKg), 3)
                })
                .OrderBy(x => x.GetType().GetProperty("Nivel")?.GetValue(x, null))
                .ThenBy(x => x.GetType().GetProperty("Tipo")?.GetValue(x, null))
                .ToList();
        }
    }
}