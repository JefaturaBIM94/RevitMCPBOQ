using System;
using System.Collections.Generic;
using System.Linq;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Aggregation
{
    public class BoqAggregationService
    {
        public List<BoqSummaryRow> AggregateBoqRows(IEnumerable<BoqRow> rows)
        {
            var result = new Dictionary<string, BoqSummaryRow>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows ?? Enumerable.Empty<BoqRow>())
            {
                if (row == null) continue;

                string key = string.Join("|",
                    row.Nivel ?? "",
                    row.Categoria ?? "",
                    row.Familia ?? "",
                    row.Tipo ?? "",
                    row.Unidad ?? "",
                    row.UbicacionEstructural ?? "");

                BoqSummaryRow bucket;
                if (!result.TryGetValue(key, out bucket))
                {
                    bucket = new BoqSummaryRow
                    {
                        Nivel = row.Nivel ?? "",
                        Cat = row.Categoria ?? "",
                        Familia = row.Familia ?? "",
                        Tipo = row.Tipo ?? "",
                        TipoDesc = row.TipoDesc ?? "",
                        TipoMaterial = row.TipoMaterial ?? "",
                        TipoAncho = row.TipoAncho,
                        TipoEspesor = row.TipoEspesor,
                        Unidad = row.Unidad ?? "",
                        UbicacionEstructural = row.UbicacionEstructural ?? "",
                        N = 0
                    };
                    result[key] = bucket;
                }

                bucket.N++;
                bucket.Area += row.Area;
                bucket.Vol += row.Volumen;
                bucket.Long_ += row.Longitud;
                bucket.Cantidad += row.Cantidad;
            }

            return result.Values
                .OrderBy(x => x.Cat)
                .ThenBy(x => x.Nivel)
                .ThenBy(x => x.Tipo)
                .ToList();
        }
    }
}