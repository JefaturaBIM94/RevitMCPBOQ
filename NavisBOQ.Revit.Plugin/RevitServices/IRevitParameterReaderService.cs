using Autodesk.Revit.DB;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public interface IRevitParameterReaderService
    {
        // ------------------------------------------------------------
        // Identidad / base
        // ------------------------------------------------------------
        string ReadCategory(Element element);
        string ReadCategoryId(Element element);
        string ReadFamily(Element element, ElementType elementType);
        string ReadType(Element element, ElementType elementType);
        string ReadMark(Element element);

        // ------------------------------------------------------------
        // Geometría base
        // ------------------------------------------------------------
        double ReadLengthM(Element element);
        double ReadAreaM2(Element element);
        double ReadVolumeM3(Element element);

        // ------------------------------------------------------------
        // Eléctrico
        // ------------------------------------------------------------
        string ReadPanelName(Element element);
        string ReadElectricalData(Element element);
        string ReadMainBreakerPower(Element element);
        string ReadCustomPartida(Element element);
        string ReadSizeText(Element element);

        // ------------------------------------------------------------
        // Sistema MEP
        // ------------------------------------------------------------
        string ReadSystemName(Element element);
        string ReadSystemType(Element element);
        string ReadSystemClassification(Element element);

        // ------------------------------------------------------------
        // Type / metadata
        // ------------------------------------------------------------
        string ReadOmniClassTitle(Element element, ElementType elementType);
        string ReadTypeDescription(Element element, ElementType elementType);
        string ReadLoadClassification(Element element, ElementType elementType);
        string ReadKeynote(Element element, ElementType elementType);
        string ReadTypeComments(Element element, ElementType elementType);
        string ReadUrl(Element element, ElementType elementType);
        double ReadDimensionA(Element element, ElementType elementType);
        double ReadDimensionB(Element element, ElementType elementType);

        // ------------------------------------------------------------
        // Acero / estructura metálica
        // ------------------------------------------------------------
        double ReadNominalWeightKgm(Element element, ElementType elementType);
        double ReadLinearWeightKgm(Element element, ElementType elementType);
        string ReadSectionName(Element element, ElementType elementType);
        string ReadSectionShape(Element element, ElementType elementType);
        string ReadCodeName(Element element, ElementType elementType);
        string ReadCustomWeightRaw(Element element, ElementType elementType);
        string ReadTypeMaterial(Element element, ElementType elementType);

        // ------------------------------------------------------------
        // HVAC / FPS / Piping
        // ------------------------------------------------------------
        string ReadOverallSizeText(Element element);
        string ReadFreeSizeText(Element element);

        double ReadWidthM(Element element);
        double ReadHeightM(Element element);
        double ReadDiameterM(Element element);
        double ReadEquivalentDiameterM(Element element);
        double ReadOutsideDiameterM(Element element, ElementType elementType);

        double ReadDuctWidthM(Element element);
        double ReadDuctHeightM(Element element);
        double ReadDuctWidth1M(Element element);
        double ReadDuctHeight1M(Element element);
        double ReadDuctWidth2M(Element element);
        double ReadDuctHeight2M(Element element);
        double ReadDuctLengthM(Element element);
        double ReadDuctLength1M(Element element);

        double ReadWidthOffsetM(Element element);
        double ReadHeightOffsetM(Element element);
        double ReadCenterRadiusM(Element element);
        double ReadAngleDeg(Element element);

        string ReadPipeSegment(Element element, ElementType elementType);
        string ReadPipeSegmentDescription(Element element, ElementType elementType);
        double ReadWallThicknessM(Element element);

        double ReadFlowValue(Element element);
        double ReadVelocityValue(Element element);
        double ReadPressureValue(Element element);
        double ReadLossCoefficient(Element element);

        string ReadInsulationType(Element element);
        double ReadInsulationThicknessM(Element element);
        string ReadLiningType(Element element);
        double ReadLiningThicknessM(Element element);

        // ------------------------------------------------------------
        // HVAC fitting logic / kg / geometría auxiliar
        // ------------------------------------------------------------
        string ReadSheetMetalKgRawText(Element element, ElementType elementType);
        double ReadSheetMetalKgRaw(Element element, ElementType elementType);
        string ReadFittingSubcategory(Element element, ElementType elementType);

        double ReadPieceBaseM(Element element);
        double ReadPieceHeightM(Element element);
        double ReadReportingAngleDeg(Element element);
    }
}