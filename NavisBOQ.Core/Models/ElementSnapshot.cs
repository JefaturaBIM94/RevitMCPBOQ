namespace NavisBOQ.Core.Models
{
    public class ElementSnapshot
    {
        public string CanonicalId { get; set; } = "";
        public string ElementId { get; set; } = "";
        public string UniqueId { get; set; } = "";

        public string Level { get; set; } = "Sin nivel";
        public string Category { get; set; } = "";
        public string CategoryId { get; set; } = "";
        public string Family { get; set; } = "";
        public string Type { get; set; } = "";
        public string Material { get; set; } = "";
        public string Mark { get; set; } = "";

        public double LengthM { get; set; }
        public double AreaM2 { get; set; }
        public double VolumeM3 { get; set; }
        public double CutLengthM { get; set; }

        public string TypeDesc { get; set; } = "";
        public string TypeMaterial { get; set; } = "";
        public double TypeWidth { get; set; }
        public double TypeThickness { get; set; }

        public double NominalWeightKgm { get; set; }
        public double LinearWeightKgm { get; set; }
        public double DepthM { get; set; }
        public double WidthXM { get; set; }
        public string SectionName { get; set; } = "";
        public string SectionShape { get; set; } = "";
        public string CodeName { get; set; } = "";

        public string SystemName { get; set; } = "Sin sistema MEP";
        public string SystemType { get; set; } = "";
        public string SystemClassification { get; set; } = "";
        public string ElectricalData { get; set; } = "";
        public string PanelName { get; set; } = "";
        public string MainBreakerPower { get; set; } = "";
        public string CustomPartida { get; set; } = "";
        public string OmniClassTitle { get; set; } = "";
        public string PieceType { get; set; } = "";
        public string SizeText { get; set; } = "";
        public double LengthByInstanceM { get; set; }
        public string FamilyTypeName { get; set; } = "";
        public string TypeNodeName { get; set; } = "";
        public string CategoryDisplay { get; set; } = "";
        public string LoadClassification { get; set; } = "";
        public string KeynoteNote { get; set; } = "";
        public string TypeComments { get; set; } = "";
        public string Url { get; set; } = "";
        public string PanelInstance { get; set; } = "";
        public double DimensionA { get; set; }
        public double DimensionB { get; set; }

        public string SourceSystem { get; set; } = "Revit";
        public string CustomWeightRaw { get; set; } = "";

        public string ResolvedFrom { get; set; } = "instance";
        public string LevelSource { get; set; } = "native";
        public string GeometryConfidence { get; set; } = "high";
        public bool NestedFamilyDetected { get; set; }
        public bool PartialData { get; set; }

        // HVAC / MEP system
  

        // HVAC geometry / sizing
       
        public string OverallSizeText { get; set; } = "";
        public string FreeSizeText { get; set; } = "";


        public double DimensionC { get; set; }

        public double DiameterM { get; set; }
        public double EquivalentDiameterM { get; set; }

        // Duct fitting specific
        public double DuctWidthM { get; set; }
        public double DuctHeightM { get; set; }
        public double DuctWidth1M { get; set; }
        public double DuctHeight1M { get; set; }
        public double DuctWidth2M { get; set; }
        public double DuctHeight2M { get; set; }
        public double DuctLengthM { get; set; }
        public double DuctLength1M { get; set; }

        public double WidthOffsetM { get; set; }
        public double HeightOffsetM { get; set; }
        public double CenterRadiusM { get; set; }
        public double AngleDeg { get; set; }

        // Pipe
        public string SegmentName { get; set; } = "";
        public string SegmentDescription { get; set; } = "";
        public double WallThicknessM { get; set; }

        // HVAC performance
        public double FlowValue { get; set; }
        public double VelocityValue { get; set; }
        public double PressureValue { get; set; }
        public double LossCoefficient { get; set; }

        // Insulation
        public string InsulationType { get; set; } = "";
        public double InsulationThicknessM { get; set; }
        public string InteriorInsulationType { get; set; } = "";
        public double InteriorInsulationThicknessM { get; set; }

        // Diagnostic / raw helpers
        public string RawCategory { get; set; } = "";
        public string RawTypeName { get; set; } = "";
        public string RawFamilyName { get; set; } = "";

        public string FittingSubcategory { get; set; } = "";
        public string SheetMetalKgRawText { get; set; } = "";
        public double SheetMetalKgRaw { get; set; }
        public bool HasSheetMetalKgRaw { get; set; }

        public double PieceBaseM { get; set; }
        public double PieceHeightM { get; set; }
        public double ReportingAngleDeg { get; set; }

        public double OutsideDiameterM { get; set; }



    }
}