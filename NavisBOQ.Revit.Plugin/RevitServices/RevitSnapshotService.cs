using Autodesk.Revit.DB;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public class RevitSnapshotService : IRevitSnapshotService
    {
        private readonly IRevitParameterReaderService _parameterReader;

        public RevitSnapshotService(IRevitParameterReaderService parameterReader)
        {
            _parameterReader = parameterReader;
        }

        public ElementSnapshot BuildSnapshot(Document document, Element element)
        {
            if (document == null || element == null)
                return null;

            ElementType elementType = document.GetElement(element.GetTypeId()) as ElementType;
            string fittingSubcategory = _parameterReader.ReadFittingSubcategory(element, elementType);
            string sheetMetalKgRawText = _parameterReader.ReadSheetMetalKgRawText(element, elementType);
            double sheetMetalKgRaw = _parameterReader.ReadSheetMetalKgRaw(element, elementType);

            var snapshot = new ElementSnapshot
            {
                CanonicalId = element.UniqueId ?? "",
                ElementId = element.Id != null ? element.Id.Value.ToString() : "",
                UniqueId = element.UniqueId ?? "",

                Category = _parameterReader.ReadCategory(element),
                CategoryId = _parameterReader.ReadCategoryId(element),
                Family = _parameterReader.ReadFamily(element, elementType),
                Type = _parameterReader.ReadType(element, elementType),
                Mark = _parameterReader.ReadMark(element),

                LengthM = _parameterReader.ReadLengthM(element),
                AreaM2 = _parameterReader.ReadAreaM2(element),
                VolumeM3 = _parameterReader.ReadVolumeM3(element),

                TypeDesc = _parameterReader.ReadTypeDescription(element, elementType),
                TypeMaterial = _parameterReader.ReadTypeMaterial(element, elementType),

                SystemName = _parameterReader.ReadSystemName(element),
                SystemType = _parameterReader.ReadSystemType(element),
                SystemClassification = _parameterReader.ReadSystemClassification(element),

                ElectricalData = _parameterReader.ReadElectricalData(element),
                PanelName = _parameterReader.ReadPanelName(element),
                MainBreakerPower = _parameterReader.ReadMainBreakerPower(element),
                CustomPartida = _parameterReader.ReadCustomPartida(element),

                OmniClassTitle = _parameterReader.ReadOmniClassTitle(element, elementType),
                SizeText = _parameterReader.ReadSizeText(element),
                FamilyTypeName = _parameterReader.ReadFamily(element, elementType),
                TypeNodeName = _parameterReader.ReadType(element, elementType),
                CategoryDisplay = _parameterReader.ReadCategory(element),
                LoadClassification = _parameterReader.ReadLoadClassification(element, elementType),
                KeynoteNote = _parameterReader.ReadKeynote(element, elementType),
                TypeComments = _parameterReader.ReadTypeComments(element, elementType),
                Url = _parameterReader.ReadUrl(element, elementType),

                DimensionA = _parameterReader.ReadDimensionA(element, elementType),
                DimensionB = _parameterReader.ReadDimensionB(element, elementType),

                NominalWeightKgm = _parameterReader.ReadNominalWeightKgm(element, elementType),
                LinearWeightKgm = _parameterReader.ReadLinearWeightKgm(element, elementType),
                SectionName = _parameterReader.ReadSectionName(element, elementType),
                SectionShape = _parameterReader.ReadSectionShape(element, elementType),
                CodeName = _parameterReader.ReadCodeName(element, elementType),
                CustomWeightRaw = _parameterReader.ReadCustomWeightRaw(element, elementType),

                // ============================================================
                // HVAC / MEP
                // ============================================================
                OverallSizeText = _parameterReader.ReadOverallSizeText(element),
                FreeSizeText = _parameterReader.ReadFreeSizeText(element),

                DiameterM = _parameterReader.ReadDiameterM(element),
                EquivalentDiameterM = _parameterReader.ReadEquivalentDiameterM(element),

                DuctWidthM = _parameterReader.ReadDuctWidthM(element),
                DuctHeightM = _parameterReader.ReadDuctHeightM(element),
                DuctWidth1M = _parameterReader.ReadDuctWidth1M(element),
                DuctHeight1M = _parameterReader.ReadDuctHeight1M(element),
                DuctWidth2M = _parameterReader.ReadDuctWidth2M(element),
                DuctHeight2M = _parameterReader.ReadDuctHeight2M(element),
                DuctLengthM = _parameterReader.ReadDuctLengthM(element),
                DuctLength1M = _parameterReader.ReadDuctLength1M(element),

                WidthOffsetM = _parameterReader.ReadWidthOffsetM(element),
                HeightOffsetM = _parameterReader.ReadHeightOffsetM(element),
                CenterRadiusM = _parameterReader.ReadCenterRadiusM(element),
                AngleDeg = _parameterReader.ReadAngleDeg(element),

                SegmentName = _parameterReader.ReadPipeSegment(element, elementType),
                SegmentDescription = _parameterReader.ReadPipeSegmentDescription(element, elementType),
                WallThicknessM = _parameterReader.ReadWallThicknessM(element),

                FlowValue = _parameterReader.ReadFlowValue(element),
                VelocityValue = _parameterReader.ReadVelocityValue(element),
                PressureValue = _parameterReader.ReadPressureValue(element),
                LossCoefficient = _parameterReader.ReadLossCoefficient(element),

                InsulationType = _parameterReader.ReadInsulationType(element),
                InsulationThicknessM = _parameterReader.ReadInsulationThicknessM(element),
                InteriorInsulationType = _parameterReader.ReadLiningType(element),
                InteriorInsulationThicknessM = _parameterReader.ReadLiningThicknessM(element),

                RawCategory = _parameterReader.ReadCategory(element),
                RawFamilyName = _parameterReader.ReadFamily(element, elementType),
                RawTypeName = _parameterReader.ReadType(element, elementType),

                SourceSystem = "Revit",
                ResolvedFrom = "instance",
                LevelSource = "native",
                GeometryConfidence = "high",
                NestedFamilyDetected = false,
                PartialData = false,

                FittingSubcategory = fittingSubcategory,
                SheetMetalKgRawText = sheetMetalKgRawText,
                SheetMetalKgRaw = sheetMetalKgRaw,
                HasSheetMetalKgRaw = sheetMetalKgRaw > 0,

                PieceBaseM = _parameterReader.ReadPieceBaseM(element),
                PieceHeightM = _parameterReader.ReadPieceHeightM(element),
                ReportingAngleDeg = _parameterReader.ReadReportingAngleDeg(element),


            };

            snapshot.Level = ResolveLevel(document, element);

            if (snapshot.LengthByInstanceM <= 0)
                snapshot.LengthByInstanceM = snapshot.LengthM;

            if (string.IsNullOrWhiteSpace(snapshot.SystemName))
                snapshot.SystemName = "Sin sistema MEP";

            if (string.IsNullOrWhiteSpace(snapshot.SystemClassification))
                snapshot.SystemClassification = "Sin sistema MEP";

            // ------------------------------------------------------------
            // Fallbacks HVAC simples
            // ------------------------------------------------------------
            if (snapshot.DimensionA <= 0)
                snapshot.DimensionA = _parameterReader.ReadWidthM(element);

            if (snapshot.DimensionB <= 0)
                snapshot.DimensionB = _parameterReader.ReadHeightM(element);

            if (snapshot.LengthM <= 0)
                snapshot.LengthM = snapshot.LengthByInstanceM;

            // Si no vino material de tipo, intenta dejarlo desde comentarios
            if (string.IsNullOrWhiteSpace(snapshot.TypeMaterial) &&
                !string.IsNullOrWhiteSpace(snapshot.TypeComments))
            {
                snapshot.TypeMaterial = snapshot.TypeComments;
            }

            return snapshot;
        }

        private string ResolveLevel(Document document, Element element)
        {
            if (document == null || element == null)
                return "Sin nivel";

            Parameter levelParam = element.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (levelParam != null && levelParam.StorageType == StorageType.ElementId)
            {
                ElementId levelId = levelParam.AsElementId();
                if (levelId != null && levelId != ElementId.InvalidElementId)
                {
                    Level level = document.GetElement(levelId) as Level;
                    if (level != null)
                        return level.Name ?? "Sin nivel";
                }
            }

            Level directLevel = document.GetElement(element.LevelId) as Level;
            if (directLevel != null)
                return directLevel.Name ?? "Sin nivel";

            return "Sin nivel";
        }
    }
}