using System;
using System.Collections.Generic;
using System.Linq;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Steel
{
    public class StructuralSteelAggregationService
    {
        public List<object> Aggregate(List<SteelRunRow> rows)
        {
            return rows
                .GroupBy(r => new
                {
                    Nivel = r.Nivel ?? "Sin nivel",
                    Categoria = r.Categoria ?? "",
                    Familia = r.Familia ?? "",
                    Tipo = r.Tipo ?? "",
                    SectionName = r.SectionName ?? "",
                    SectionShape = r.SectionShape ?? "",
                    CodeName = r.CodeName ?? "",
                    Metodo = r.Metodo ?? ""
                })
                .Select(g =>
                {
                    double totalKg = g.Sum(x => x.PesoKg);
                    return (object)new
                    {
                        Nivel = g.Key.Nivel,
                        Categoria = g.Key.Categoria,
                        Familia = g.Key.Familia,
                        Tipo = g.Key.Tipo,
                        SectionName = g.Key.SectionName,
                        SectionShape = g.Key.SectionShape,
                        CodeName = g.Key.CodeName,
                        NominalWeightKgm = Math.Round(g.Max(x => x.NominalWeightKgm), 4),
                        LinearWeightKgm = Math.Round(g.Max(x => x.LinearWeightKgm), 4),
                        NumPiezas = g.Count(),
                        LengthTotalM = Math.Round(g.Sum(x => x.LengthM), 3),
                        VolumeTotalM3 = Math.Round(g.Sum(x => x.VolumeM3), 3),
                        PesoTotalKg = Math.Round(totalKg, 2),
                        PesoTonRef = totalKg >= 1000.0 ? Math.Round(totalKg / 1000.0, 3) : (double?)null,
                        Metodo = g.Key.Metodo,
                        Advertencia = string.Join(" | ", g.Select(x => x.Advertencia).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                    };
                })
                .OrderBy(x => x.GetType().GetProperty("Categoria")?.GetValue(x, null))
                .ThenBy(x => x.GetType().GetProperty("Nivel")?.GetValue(x, null))
                .ThenBy(x => x.GetType().GetProperty("Tipo")?.GetValue(x, null))
                .ToList();
        }
    }
}