using System;
using NavisBOQ.Core.Constants;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Mapping
{
    public class QuantityMapperService
    {
        public BoqRow ToBoqRow(ElementSnapshot snap)
        {
            var row = new BoqRow
            {
                Nivel = snap.Level ?? "Sin nivel",
                Familia = Clean(snap.Family),
                Tipo = Clean(snap.Type),
                TipoDesc = snap.TypeDesc ?? "",
                TipoMaterial = !string.IsNullOrWhiteSpace(snap.TypeMaterial) ? snap.TypeMaterial : (snap.Material ?? ""),
                TipoAncho = Math.Round(snap.TypeWidth, 4),
                TipoEspesor = Math.Round(snap.TypeThickness, 4),
                Area = Math.Round(snap.AreaM2, 4),
                Volumen = Math.Round(snap.VolumeM3, 4),
                Longitud = Math.Round(snap.LengthM, 4),
                ElemId = snap.ElementId ?? "",
                UbicacionEstructural = ""
            };

            string boq = CategoryConstants.ResolveCanonicalName(snap.Category ?? "");
            string unit = CategoryConstants.ResolveDefaultUnit(boq);

            row.Categoria = boq;

            if (CategoryConstants.PieceCategories.Contains(boq))
            {
                row.Cantidad = 1;
                row.Unidad = "pza";
            }
            else if (string.Equals(unit, "m2", StringComparison.OrdinalIgnoreCase) && snap.AreaM2 > 0)
            {
                row.Cantidad = Math.Round(snap.AreaM2, 3);
                row.Unidad = "m2";
            }
            else if (string.Equals(unit, "m3", StringComparison.OrdinalIgnoreCase) && snap.VolumeM3 > 0)
            {
                row.Cantidad = Math.Round(snap.VolumeM3, 3);
                row.Unidad = "m3";
            }
            else if (string.Equals(unit, "ml", StringComparison.OrdinalIgnoreCase) && snap.LengthM > 0)
            {
                row.Cantidad = Math.Round(snap.LengthM, 3);
                row.Unidad = "ml";
            }
            else if (snap.AreaM2 > 0)
            {
                row.Cantidad = Math.Round(snap.AreaM2, 3);
                row.Unidad = "m2";
            }
            else if (snap.VolumeM3 > 0)
            {
                row.Cantidad = Math.Round(snap.VolumeM3, 3);
                row.Unidad = "m3";
            }
            else if (snap.LengthM > 0)
            {
                row.Cantidad = Math.Round(snap.LengthM, 3);
                row.Unidad = "ml";
            }
            else
            {
                row.Cantidad = 1;
                row.Unidad = "pza";
            }

            return row;
        }

        private static string Clean(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "" : s.Trim();
        }
    }
}
