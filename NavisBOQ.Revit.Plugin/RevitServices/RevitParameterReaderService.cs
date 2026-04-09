using Autodesk.Revit.DB;

using NavisBOQ.Revit.Plugin.Infrastructure;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public class RevitParameterReaderService : IRevitParameterReaderService
    {
        public string ReadCategory(Element element)
        {
            return element != null && element.Category != null ? element.Category.Name ?? "" : "";
        }

        public string ReadCategoryId(Element element)
        {
            return element != null && element.Category != null
                ? element.Category.Id.Value.ToString()
                : "";
        }

        public string ReadFamily(Element element, ElementType elementType)
        {
            var familySymbol = elementType as FamilySymbol;
            if (familySymbol != null && familySymbol.Family != null)
                return familySymbol.Family.Name ?? "";

            return elementType != null ? elementType.FamilyName ?? "" : "";
        }

        public string ReadType(Element element, ElementType elementType)
        {
            return elementType != null ? elementType.Name ?? "" : "";
        }

        public string ReadMark(Element element)
        {
            return ReadStringBuiltIn(element, BuiltInParameter.ALL_MODEL_MARK);
        }

        public double ReadLengthM(Element element)
        {
            double value;

            value = ReadDoubleBuiltIn(element, BuiltInParameter.CURVE_ELEM_LENGTH);
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleBuiltIn(element, BuiltInParameter.INSTANCE_LENGTH_PARAM);
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Length");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Longitud");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadAreaM2(Element element)
        {
            double value;

            value = ReadDoubleBuiltIn(element, BuiltInParameter.HOST_AREA_COMPUTED);
            if (value > 0) return RevitUnitUtils.ToSquareMeters(value);

            value = ReadDoubleByName(element, "Area");
            if (value > 0) return RevitUnitUtils.ToSquareMeters(value);

            value = ReadDoubleByName(element, "Área");
            if (value > 0) return RevitUnitUtils.ToSquareMeters(value);

            return 0.0;
        }

        public double ReadVolumeM3(Element element)
        {
            double value;

            value = ReadDoubleBuiltIn(element, BuiltInParameter.HOST_VOLUME_COMPUTED);
            if (value > 0) return RevitUnitUtils.ToCubicMeters(value);

            value = ReadDoubleByName(element, "Volume");
            if (value > 0) return RevitUnitUtils.ToCubicMeters(value);

            value = ReadDoubleByName(element, "Volumen");
            if (value > 0) return RevitUnitUtils.ToCubicMeters(value);

            return 0.0;
        }

        public string ReadPanelName(Element element)
        {
            string value;

            value = ReadStringByName(element, "Panel");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Panel Name");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Nombre del panel");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringBuiltIn(element, BuiltInParameter.RBS_ELEC_PANEL_NAME);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadElectricalData(Element element)
        {
            string value;

            value = ReadStringByName(element, "Electrical Data");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Datos eléctricos");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Data");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadMainBreakerPower(Element element)
        {
            string value;

            value = ReadStringByName(element, "Main Breaker");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Main Breaker Rating");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadCustomPartida(Element element)
        {
            return ReadStringByName(element, "TR3Z - Partida");
        }

        public string ReadSizeText(Element element)
        {
            string value;

            value = ReadStringByName(element, "Size");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Tamaño");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringBuiltIn(element, BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringBuiltIn(element, BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadSystemName(Element element)
        {
            if (element == null)
                return "";

            string value = ReadStringByName(element, "System Name");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Nombre de sistema");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "System");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadSystemType(Element element)
        {
            if (element == null)
                return "";

            string value = ReadStringByName(element, "System Type");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Tipo de sistema");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadSystemClassification(Element element)
        {
            string value = ReadStringByName(element, "System Classification");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Clasificación de sistema");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadOmniClassTitle(Element element, ElementType elementType)
        {
            string value = ReadStringByName(elementType, "OmniClass Title");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Título OmniClass");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadTypeDescription(Element element, ElementType elementType)
        {
            string value = ReadStringByName(elementType, "Description");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Descripción");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadLoadClassification(Element element, ElementType elementType)
        {
            string value = ReadStringByName(elementType, "Load Classification");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Clasificación de carga");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadKeynote(Element element, ElementType elementType)
        {
            return ReadStringBuiltIn(elementType, BuiltInParameter.KEYNOTE_PARAM);
        }

        public string ReadTypeComments(Element element, ElementType elementType)
        {
            return ReadStringBuiltIn(elementType, BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);
        }

        public string ReadUrl(Element element, ElementType elementType)
        {
            return ReadStringBuiltIn(elementType, BuiltInParameter.ALL_MODEL_URL);
        }

        public double ReadDimensionA(Element element, ElementType elementType)
        {
            double value;

            value = ReadDoubleByName(elementType, "A");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(elementType, "Ancho");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDimensionB(Element element, ElementType elementType)
        {
            double value;

            value = ReadDoubleByName(elementType, "B");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(elementType, "Altura");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadNominalWeightKgm(Element element, ElementType elementType)
        {
            return 0.0;
        }

        public double ReadLinearWeightKgm(Element element, ElementType elementType)
        {
            return 0.0;
        }

        public string ReadSectionName(Element element, ElementType elementType)
        {
            return ReadStringByName(elementType, "Section Name");
        }

        public string ReadSectionShape(Element element, ElementType elementType)
        {
            return ReadStringByName(elementType, "Section Shape");
        }

        public string ReadCodeName(Element element, ElementType elementType)
        {
            return ReadStringByName(elementType, "Code Name");
        }

        public string ReadCustomWeightRaw(Element element, ElementType elementType)
        {
            return ReadStringByName(elementType, "VDC_WEIGHT");
        }

        public string ReadTypeMaterial(Element element, ElementType elementType)
        {
            string value = ReadStringByName(elementType, "Structural Material");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Material");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        // ============================================================
        // HVAC / MEP
        // ============================================================

        public string ReadOverallSizeText(Element element)
        {
            string value = ReadStringByName(element, "Overall Size");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Tamaño total");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadFreeSizeText(Element element)
        {
            string value = ReadStringByName(element, "Free Size");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Tamaño libre");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public double ReadWidthM(Element element)
        {
            double value;

            value = ReadDoubleByName(element, "Width");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Anchura");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Ancho");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadHeightM(Element element)
        {
            double value;

            value = ReadDoubleByName(element, "Height");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Altura");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDiameterM(Element element)
        {
            double value;

            value = ReadDoubleByName(element, "Diameter");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Diámetro");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Nominal Diameter");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadEquivalentDiameterM(Element element)
        {
            double value;

            value = ReadDoubleByName(element, "Equivalent Diameter");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Diámetro equivalente");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctWidthM(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Width");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctHeightM(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Height");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctWidth1M(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Width 1");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctHeight1M(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Height 1");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctWidth2M(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Width 2");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctHeight2M(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Height 2");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctLengthM(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Length");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadDuctLength1M(Element element)
        {
            double value = ReadDoubleByName(element, "Duct Length 1");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadWidthOffsetM(Element element)
        {
            double value = ReadDoubleByName(element, "Width Offset");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Anchura de desfase");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadHeightOffsetM(Element element)
        {
            double value = ReadDoubleByName(element, "Height Offset");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Altura de desfase");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadCenterRadiusM(Element element)
        {
            double value = ReadDoubleByName(element, "Center Radius");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadAngleDeg(Element element)
        {
            double value = ReadDoubleByName(element, "Angle");
            if (value > 0) return RevitUnitUtils.ToDegrees(value);

            value = ReadDoubleByName(element, "Ángulo");
            if (value > 0) return RevitUnitUtils.ToDegrees(value);

            return 0.0;
        }

        public string ReadPipeSegment(Element element, ElementType elementType)
        {
            string value = ReadStringByName(element, "Pipe Segment");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Segmento de tubería");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Pipe Segment");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Segmento de tubería");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public string ReadPipeSegmentDescription(Element element, ElementType elementType)
        {
            string value = ReadStringByName(element, "Segment Description");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Descripción de segmento");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Segment Description");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Descripción de segmento");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public double ReadWallThicknessM(Element element)
        {
            double value = ReadDoubleByName(element, "Wall Thickness");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Espesor de pared");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadFlowValue(Element element)
        {
            double value = ReadDoubleByName(element, "Flow");
            if (value != 0) return value;

            value = ReadDoubleByName(element, "Caudal");
            if (value != 0) return value;

            return 0.0;
        }

        public double ReadVelocityValue(Element element)
        {
            double value = ReadDoubleByName(element, "Velocity");
            if (value != 0) return value;

            value = ReadDoubleByName(element, "Velocidad");
            if (value != 0) return value;

            return 0.0;
        }

        public double ReadPressureValue(Element element)
        {
            double value = ReadDoubleByName(element, "Pressure");
            if (value != 0) return value;

            value = ReadDoubleByName(element, "Presión");
            if (value != 0) return value;

            return 0.0;
        }

        public double ReadLossCoefficient(Element element)
        {
            double value = ReadDoubleByName(element, "Loss Coefficient");
            if (value != 0) return value;

            value = ReadDoubleByName(element, "Coeficiente de pérdida");
            if (value != 0) return value;

            return 0.0;
        }

        public string ReadInsulationType(Element element)
        {
            string value = ReadStringByName(element, "Insulation Type");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Tipo de aislamiento");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public double ReadInsulationThicknessM(Element element)
        {
            double value = ReadDoubleByName(element, "Insulation Thickness");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Grosor de aislamiento");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public string ReadLiningType(Element element)
        {
            string value = ReadStringByName(element, "Lining Type");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Tipo de aislamiento interior");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public double ReadLiningThicknessM(Element element)
        {
            double value = ReadDoubleByName(element, "Lining Thickness");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Grosor de aislamiento interior");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        private string ReadStringBuiltIn(Element element, BuiltInParameter bip)
        {
            if (element == null)
                return "";

            Parameter p = element.get_Parameter(bip);
            if (p == null)
                return "";

            return ReadStringParameter(p);
        }

        private string ReadStringByName(Element element, string parameterName)
        {
            if (element == null || string.IsNullOrWhiteSpace(parameterName))
                return "";

            Parameter p = element.LookupParameter(parameterName);
            if (p == null)
                return "";

            return ReadStringParameter(p);
        }

        private string ReadStringParameter(Parameter parameter)
        {
            if (parameter == null)
                return "";

            if (parameter.StorageType == StorageType.String)
                return parameter.AsString() ?? "";

            string value = parameter.AsValueString();
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        private double ReadDoubleBuiltIn(Element element, BuiltInParameter bip)
        {
            if (element == null)
                return 0.0;

            Parameter p = element.get_Parameter(bip);
            return ReadDoubleParameter(p);
        }

        private double ReadDoubleByName(Element element, string parameterName)
        {
            if (element == null || string.IsNullOrWhiteSpace(parameterName))
                return 0.0;

            Parameter p = element.LookupParameter(parameterName);
            return ReadDoubleParameter(p);
        }

        private double ReadDoubleParameter(Parameter parameter)
        {
            if (parameter == null)
                return 0.0;

            if (parameter.StorageType == StorageType.Double)
                return parameter.AsDouble();

            return 0.0;
        }

        public string ReadSheetMetalKgRawText(Element element, ElementType elementType)
        {
            string value = ReadStringByName(element, "Kilogramos de Lámina");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Kilogramos de lamina");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(element, "Sheet Metal Kilograms");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Kilogramos de Lámina");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            value = ReadStringByName(elementType, "Sheet Metal Kilograms");
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return "";
        }

        public double ReadSheetMetalKgRaw(Element element, ElementType elementType)
        {
            double value = ReadDoubleByName(element, "Kilogramos de Lámina");
            if (value > 0)
                return value;

            value = ReadDoubleByName(element, "Kilogramos de lamina");
            if (value > 0)
                return value;

            value = ReadDoubleByName(element, "Sheet Metal Kilograms");
            if (value > 0)
                return value;

            value = ReadDoubleByName(elementType, "Kilogramos de Lámina");
            if (value > 0)
                return value;

            value = ReadDoubleByName(elementType, "Sheet Metal Kilograms");
            if (value > 0)
                return value;

            return 0.0;
        }

        public string ReadFittingSubcategory(Element element, ElementType elementType)
        {
            string text = ((ReadType(element, elementType) ?? "") + " " + (ReadFamily(element, elementType) ?? "")).ToLowerInvariant();

            if (text.Contains("transition") || text.Contains("reduccion") || text.Contains("reduction"))
                return "Transition";

            if (text.Contains("elbow") || text.Contains("codo"))
                return "Elbow";

            if (text.Contains("tee"))
                return "Tee";

            if (text.Contains("tap"))
                return "Tap";

            if (text.Contains("cross"))
                return "Cross";

            return "GenericFitting";
        }

        public double ReadPieceBaseM(Element element)
        {
            double value = ReadDoubleByName(element, "Base de Pieza");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Piece Base");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadPieceHeightM(Element element)
        {
            double value = ReadDoubleByName(element, "Altura de Pieza");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            value = ReadDoubleByName(element, "Piece Height");
            if (value > 0) return RevitUnitUtils.ToMeters(value);

            return 0.0;
        }

        public double ReadReportingAngleDeg(Element element)
        {
            double value = ReadDoubleByName(element, "Reporting Angle");
            if (value > 0) return RevitUnitUtils.ToDegrees(value);

            value = ReadDoubleByName(element, "Ángulo de reporte");
            if (value > 0) return RevitUnitUtils.ToDegrees(value);

            return 0.0;
        }


    }
}