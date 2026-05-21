using NavisBOQ.Core.Constants;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Electrical
{
    public class ElectricalQuantityMapperService
    {
        public ElectricalRunRow Map(ElementSnapshot snap, string boqCategory, string unit)
        {
            var row = new ElectricalRunRow
            {
                Nivel = snap.Level ?? "Sin nivel",
                Sistema = string.IsNullOrWhiteSpace(snap.SystemName) ? "Sin sistema MEP" : snap.SystemName,
                CategoriaBoq = boqCategory,
                CategoriaRevit = snap.Category ?? "",
                Familia = snap.Family ?? "",
                Tipo = snap.Type ?? "",
                Descripcion = snap.TypeDesc ?? "",
                OmniClassTitle = snap.OmniClassTitle ?? "",
                PanelName = snap.PanelName ?? "",
                ElectricalData = snap.ElectricalData ?? "",
                PartidaCustom = snap.CustomPartida ?? "",
                ElemId = snap.ElementId ?? "",

                FamilyTypeName = snap.FamilyTypeName ?? "",
                TypeNodeName = snap.TypeNodeName ?? "",
                CategoryDisplay = snap.CategoryDisplay ?? "",
                LoadClassification = snap.LoadClassification ?? "",
                KeynoteNote = snap.KeynoteNote ?? "",
                TypeComments = snap.TypeComments ?? "",
                Url = snap.Url ?? "",
                PieceType = snap.PieceType ?? "",
                MainBreakerPower = snap.MainBreakerPower ?? "",
                PanelInstance = snap.PanelInstance ?? "",
                SizeText = snap.SizeText ?? "",
                LengthByInstanceMl = snap.LengthByInstanceM,
                DimensionA = snap.DimensionA,
                DimensionB = snap.DimensionB,

                Unidad = unit
            };

            if (ElectricalCategoryConstants.IsLinearCategory(snap.Category))
            {
                double length = snap.LengthByInstanceM > 0 ? snap.LengthByInstanceM : snap.LengthM;

                row.Cantidad = length;
                row.LongitudTotalMl = length;
                row.NumTramos = 1;
                row.Unidad = "ml";
            }
            else
            {
                row.Cantidad = 1;
                row.Unidad = "pza";
                row.LongitudTotalMl = 0;
                row.NumTramos = 0;
            }

            return row;
        }
    }
}
