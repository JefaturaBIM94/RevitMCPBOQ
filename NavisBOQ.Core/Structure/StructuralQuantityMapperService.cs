using System;
using NavisBOQ.Core.Constants;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Structure
{
    public class StructuralQuantityMapperService
    {
        public BoqRow ToBoqRow(ElementSnapshot snap)
        {
            if (snap == null)
                return null;

            string rawCategory = snap.Category ?? "";
            string categoria = CategoryConstants.ResolveCanonicalName(rawCategory);
            string unidad = CategoryConstants.ResolveDefaultUnit(rawCategory);

            double area = Math.Round(snap.AreaM2, 4);
            double volumen = Math.Round(snap.VolumeM3, 4);
            double longitud = Math.Round(snap.LengthM, 4);

            double cantidad = ResolveStructuralQuantity(rawCategory, area, volumen, longitud, unidad);
            string unidadFinal = ResolveStructuralUnit(rawCategory, area, volumen, longitud, unidad);

            return new BoqRow
            {
                Nivel = string.IsNullOrWhiteSpace(snap.Level) ? "Sin nivel" : snap.Level,
                Categoria = categoria,
                Familia = snap.Family ?? "",
                Tipo = snap.Type ?? "",
                TipoDesc = snap.TypeDesc ?? "",
                TipoMaterial = !string.IsNullOrWhiteSpace(snap.TypeMaterial) ? snap.TypeMaterial : (snap.Material ?? ""),
                TipoAncho = Math.Round(snap.TypeWidth, 4),
                TipoEspesor = Math.Round(snap.TypeThickness, 4),
                Area = area,
                Volumen = volumen,
                Longitud = longitud,
                Cantidad = Math.Round(cantidad, 4),
                Unidad = unidadFinal,
                ElemId = snap.ElementId ?? "",
                UbicacionEstructural = ""
            };
        }

        private static double ResolveStructuralQuantity(
            string rawCategory,
            double area,
            double volumen,
            double longitud,
            string defaultUnit)
        {
            if (IsFoundationCategory(rawCategory))
            {
                if (volumen > 0) return volumen;
                if (area > 0) return area;
                if (longitud > 0) return longitud;
                return 1;
            }

            if (IsFramingCategory(rawCategory) || IsColumnCategory(rawCategory))
            {
                if (longitud > 0) return longitud;
                if (volumen > 0) return volumen;
                if (area > 0) return area;
                return 1;
            }

            if (IsWallFloorRoofCategory(rawCategory))
            {
                if (defaultUnit == "m3" && volumen > 0) return volumen;
                if (defaultUnit == "m2" && area > 0) return area;
                if (volumen > 0) return volumen;
                if (area > 0) return area;
                if (longitud > 0) return longitud;
                return 1;
            }

            if (volumen > 0) return volumen;
            if (area > 0) return area;
            if (longitud > 0) return longitud;
            return 1;
        }

        private static string ResolveStructuralUnit(
            string rawCategory,
            double area,
            double volumen,
            double longitud,
            string defaultUnit)
        {
            if (IsFoundationCategory(rawCategory))
            {
                if (volumen > 0) return "m3";
                if (area > 0) return "m2";
                if (longitud > 0) return "ml";
                return defaultUnit;
            }

            if (IsFramingCategory(rawCategory) || IsColumnCategory(rawCategory))
            {
                if (longitud > 0) return "ml";
                if (volumen > 0) return "m3";
                if (area > 0) return "m2";
                return defaultUnit;
            }

            if (IsWallFloorRoofCategory(rawCategory))
            {
                if (defaultUnit == "m3" && volumen > 0) return "m3";
                if (defaultUnit == "m2" && area > 0) return "m2";
                if (volumen > 0) return "m3";
                if (area > 0) return "m2";
                if (longitud > 0) return "ml";
                return defaultUnit;
            }

            return defaultUnit;
        }

        private static bool IsFoundationCategory(string category)
        {
            return EqualsAny(category,
                "Structural Foundations",
                "Cimentaciones",
                "Pads",
                "Pedestales",
                "Zapatas");
        }

        private static bool IsFramingCategory(string category)
        {
            return EqualsAny(category,
                "Structural Framing",
                "Vigas estructurales");
        }

        private static bool IsColumnCategory(string category)
        {
            return EqualsAny(category,
                "Structural Columns",
                "Columnas estructurales");
        }

        private static bool IsWallFloorRoofCategory(string category)
        {
            return EqualsAny(category,
                "Walls", "Muros",
                "Floors", "Suelos", "Losas",
                "Roofs", "Cubiertas");
        }

        private static bool EqualsAny(string source, params string[] values)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            foreach (var value in values)
            {
                if (string.Equals(source.Trim(), value, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}