using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Revit.Plugin.RevitServices
{
    public class RevitSnapshotService : IRevitSnapshotService
    {
        private readonly IRevitParameterReaderService _parameterReader;

        private readonly Dictionary<long, TypePropertyBag> _typeCache =
            new Dictionary<long, TypePropertyBag>();

        public RevitSnapshotService(IRevitParameterReaderService parameterReader)
        {
            _parameterReader = parameterReader;
        }

        public ElementSnapshot BuildSnapshot(
            Document document,
            Element element,
            SnapshotReadOptions readOptions = null)
        {
            if (document == null || element == null)
                return null;

            readOptions = readOptions ?? new SnapshotReadOptions();

            ElementType elementType = null;
            if (element.GetTypeId() != ElementId.InvalidElementId)
                elementType = document.GetElement(element.GetTypeId()) as ElementType;

            var snapshot = new ElementSnapshot
            {
                CanonicalId = element.UniqueId ?? "",
                ElementId = element.Id != null ? element.Id.Value.ToString() : "",
                UniqueId = element.UniqueId ?? "",
                SourceSystem = "Revit"
            };

            if (readOptions.IncludeIdentity)
            {
                snapshot.Category = ResolveCategory(element);
                snapshot.CategoryId = _parameterReader.ReadCategoryId(element);
                snapshot.Family = _parameterReader.ReadFamily(element, elementType);
                snapshot.Type = _parameterReader.ReadType(element, elementType);
                snapshot.Mark = _parameterReader.ReadMark(element);
                snapshot.Level = ResolveLevel(document, element);

                snapshot.RawCategory = element.Category != null ? (element.Category.Name ?? "") : "";
                snapshot.RawFamilyName = snapshot.Family;
                snapshot.RawTypeName = snapshot.Type;
            }

            if (readOptions.IncludeGeometry)
            {
                snapshot.LengthM = _parameterReader.ReadLengthM(element);
                snapshot.AreaM2 = _parameterReader.ReadAreaM2(element);
                snapshot.VolumeM3 = _parameterReader.ReadVolumeM3(element);

                // Fallback robusto para Corrida 3:
                // si la lectura geométrica genérica no resuelve, intenta por nombre visible del parámetro.
                if (snapshot.LengthM <= 0)
                {
                    double lengthFallback;
                    if (TryReadLengthParameterAsMeters(element, "Length", out lengthFallback) && lengthFallback > 0)
                        snapshot.LengthM = lengthFallback;
                }

                if (snapshot.VolumeM3 <= 0)
                {
                    double volumeFallback;
                    if (TryReadVolumeParameterAsCubicMeters(element, "Volume", out volumeFallback) && volumeFallback > 0)
                        snapshot.VolumeM3 = volumeFallback;
                }

                snapshot.LengthByInstanceM = snapshot.LengthM;
            }

            if (readOptions.IncludeTypeData && elementType != null)
            {
                var bag = GetOrCreateTypeBag(element, elementType);

                snapshot.TypeDesc = bag.TypeDescription ?? "";
                snapshot.TypeMaterial = bag.StructuralMaterial ?? "";
                snapshot.TypeWidth = bag.WidthM ?? 0.0;
                snapshot.TypeThickness = bag.ThicknessM ?? 0.0;

                snapshot.OmniClassTitle = bag.FamilyTypeName ?? "";
                snapshot.FamilyTypeName = bag.FamilyTypeName ?? "";
                snapshot.TypeNodeName = bag.TypeNodeName ?? "";
                snapshot.CategoryDisplay = bag.CategoryDisplay ?? "";
                snapshot.LoadClassification = bag.LoadClassification ?? "";
                snapshot.KeynoteNote = bag.KeynoteNote ?? "";
                snapshot.TypeComments = bag.TypeComments ?? "";
                snapshot.Url = bag.Url ?? "";
                snapshot.DimensionA = bag.DimensionA ?? 0.0;
                snapshot.DimensionB = bag.DimensionB ?? 0.0;
            }

            if (readOptions.IncludeSystemData)
            {
                snapshot.SystemName = _parameterReader.ReadSystemName(element);
                snapshot.SystemType = _parameterReader.ReadSystemType(element);
                snapshot.SystemClassification = _parameterReader.ReadSystemClassification(element);

                if (string.IsNullOrWhiteSpace(snapshot.SystemName))
                    snapshot.SystemName = "Sin sistema MEP";

                if (string.IsNullOrWhiteSpace(snapshot.SystemClassification))
                    snapshot.SystemClassification = "Sin sistema MEP";
            }

            if (readOptions.IncludeElectricalData)
            {
                snapshot.ElectricalData = _parameterReader.ReadElectricalData(element);
                snapshot.PanelName = _parameterReader.ReadPanelName(element);
                snapshot.MainBreakerPower = _parameterReader.ReadMainBreakerPower(element);
                snapshot.CustomPartida = _parameterReader.ReadCustomPartida(element);
                snapshot.SizeText = _parameterReader.ReadSizeText(element);

                snapshot.PieceType = "";
                snapshot.PanelInstance = "";
            }

            if (readOptions.IncludeSteelData && elementType != null)
            {
                var bag = GetOrCreateTypeBag(element, elementType);

                snapshot.NominalWeightKgm = bag.NominalWeightKgm ?? 0.0;
                snapshot.LinearWeightKgm = bag.LinearWeightKgm ?? 0.0;
                snapshot.DepthM = bag.DepthM ?? 0.0;
                snapshot.WidthXM = bag.WidthXM ?? 0.0;

                snapshot.SectionName = bag.SectionName ?? "";
                snapshot.SectionShape = bag.SectionShape ?? "";
                snapshot.CodeName = bag.CodeName ?? "";
                snapshot.CustomWeightRaw = bag.CustomWeightRaw ?? "";
            }

            if (readOptions.IncludeHvacData)
            {
                snapshot.OverallSizeText = _parameterReader.ReadOverallSizeText(element);
                snapshot.FreeSizeText = _parameterReader.ReadFreeSizeText(element);

                snapshot.DiameterM = _parameterReader.ReadDiameterM(element);
                snapshot.EquivalentDiameterM = _parameterReader.ReadEquivalentDiameterM(element);

                snapshot.DuctWidthM = _parameterReader.ReadDuctWidthM(element);
                snapshot.DuctHeightM = _parameterReader.ReadDuctHeightM(element);
                snapshot.DuctWidth1M = _parameterReader.ReadDuctWidth1M(element);
                snapshot.DuctHeight1M = _parameterReader.ReadDuctHeight1M(element);
                snapshot.DuctWidth2M = _parameterReader.ReadDuctWidth2M(element);
                snapshot.DuctHeight2M = _parameterReader.ReadDuctHeight2M(element);
                snapshot.DuctLengthM = _parameterReader.ReadDuctLengthM(element);
                snapshot.DuctLength1M = _parameterReader.ReadDuctLength1M(element);

                snapshot.WidthOffsetM = _parameterReader.ReadWidthOffsetM(element);
                snapshot.HeightOffsetM = _parameterReader.ReadHeightOffsetM(element);
                snapshot.CenterRadiusM = _parameterReader.ReadCenterRadiusM(element);
                snapshot.AngleDeg = _parameterReader.ReadAngleDeg(element);

                snapshot.SegmentName = _parameterReader.ReadPipeSegment(element, elementType);
                snapshot.SegmentDescription = _parameterReader.ReadPipeSegmentDescription(element, elementType);
                snapshot.WallThicknessM = _parameterReader.ReadWallThicknessM(element);

                snapshot.FlowValue = _parameterReader.ReadFlowValue(element);
                snapshot.VelocityValue = _parameterReader.ReadVelocityValue(element);
                snapshot.PressureValue = _parameterReader.ReadPressureValue(element);
                snapshot.LossCoefficient = _parameterReader.ReadLossCoefficient(element);

                snapshot.InsulationType = _parameterReader.ReadInsulationType(element);
                snapshot.InsulationThicknessM = _parameterReader.ReadInsulationThicknessM(element);
                snapshot.InteriorInsulationType = _parameterReader.ReadLiningType(element);
                snapshot.InteriorInsulationThicknessM = _parameterReader.ReadLiningThicknessM(element);

                snapshot.FittingSubcategory = _parameterReader.ReadFittingSubcategory(element, elementType);
                snapshot.SheetMetalKgRawText = _parameterReader.ReadSheetMetalKgRawText(element, elementType);
                snapshot.SheetMetalKgRaw = _parameterReader.ReadSheetMetalKgRaw(element, elementType);
                snapshot.HasSheetMetalKgRaw = snapshot.SheetMetalKgRaw > 0;

                snapshot.PieceBaseM = _parameterReader.ReadPieceBaseM(element);
                snapshot.PieceHeightM = _parameterReader.ReadPieceHeightM(element);
                snapshot.ReportingAngleDeg = _parameterReader.ReadReportingAngleDeg(element);
            }

            if (readOptions.IncludeDiagnostics)
            {
                snapshot.ResolvedFrom = "instance";
                snapshot.LevelSource = "native";
                snapshot.GeometryConfidence = "high";
                snapshot.NestedFamilyDetected = false;
                snapshot.PartialData = false;
            }

            if (snapshot.LengthM <= 0)
                snapshot.LengthM = snapshot.LengthByInstanceM;

            return snapshot;
        }

        private TypePropertyBag GetOrCreateTypeBag(Element element, ElementType elementType)
        {
            long key = elementType.Id.Value;

            TypePropertyBag bag;
            if (_typeCache.TryGetValue(key, out bag))
                return bag;

            bag = new TypePropertyBag
            {
                TypeDescription = _parameterReader.ReadTypeDescription(element, elementType),
                StructuralMaterial = _parameterReader.ReadTypeMaterial(element, elementType),
                WidthM = _parameterReader.ReadDimensionA(element, elementType),
                ThicknessM = _parameterReader.ReadDimensionB(element, elementType),

                NominalWeightKgm = _parameterReader.ReadNominalWeightKgm(element, elementType),
                LinearWeightKgm = _parameterReader.ReadLinearWeightKgm(element, elementType),

                DepthM = 0.0,
                WidthXM = 0.0,

                SectionName = _parameterReader.ReadSectionName(element, elementType),
                SectionShape = _parameterReader.ReadSectionShape(element, elementType),
                CodeName = _parameterReader.ReadCodeName(element, elementType),
                CustomWeightRaw = _parameterReader.ReadCustomWeightRaw(element, elementType),

                FamilyTypeName = _parameterReader.ReadOmniClassTitle(element, elementType),
                TypeNodeName = elementType.Name ?? "",
                CategoryDisplay = element != null && element.Category != null ? element.Category.Name ?? "" : "",
                LoadClassification = _parameterReader.ReadLoadClassification(element, elementType),
                KeynoteNote = _parameterReader.ReadKeynote(element, elementType),
                TypeComments = _parameterReader.ReadTypeComments(element, elementType),
                Url = _parameterReader.ReadUrl(element, elementType),
                DimensionA = _parameterReader.ReadDimensionA(element, elementType),
                DimensionB = _parameterReader.ReadDimensionB(element, elementType)
            };

            _typeCache[key] = bag;
            return bag;
        }

        private string ResolveCategory(Element element)
        {
            if (element == null || element.Category == null)
                return "";

            int catId = element.Category.Id.IntegerValue;

            if (catId == (int)BuiltInCategory.OST_PlumbingFixtures)
                return "Plumbing Fixtures";

            if (catId == (int)BuiltInCategory.OST_SpecialityEquipment)
                return "Specialty Equipment";

            if (catId == (int)BuiltInCategory.OST_CurtainWallPanels)
                return "Curtain Wall Panels";

            if (catId == (int)BuiltInCategory.OST_Railings)
                return "Railings";

            if (catId == (int)BuiltInCategory.OST_Walls)
                return "Walls";

            if (catId == (int)BuiltInCategory.OST_Floors)
                return "Floors";

            if (catId == (int)BuiltInCategory.OST_Roofs)
                return "Roofs";

            if (catId == (int)BuiltInCategory.OST_Ceilings)
                return "Ceilings";

            if (catId == (int)BuiltInCategory.OST_Doors)
                return "Doors";

            if (catId == (int)BuiltInCategory.OST_Windows)
                return "Windows";

            if (catId == (int)BuiltInCategory.OST_GenericModel)
                return "Generic Models";

            if (catId == (int)BuiltInCategory.OST_StructuralColumns)
                return "Structural Columns";

            if (catId == (int)BuiltInCategory.OST_StructuralFraming)
                return "Structural Framing";

            if (catId == (int)BuiltInCategory.OST_StructuralFoundation)
                return "Structural Foundations";

            string rawName = element.Category.Name ?? "";

            if (string.Equals(rawName, "Structural Connections", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(rawName, "Conexiones estructurales", StringComparison.OrdinalIgnoreCase))
            {
                return "Structural Connections";
            }

            if (string.Equals(rawName, "Plates", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(rawName, "Placas", StringComparison.OrdinalIgnoreCase))
            {
                return "Plates";
            }

            if (string.Equals(rawName, "Structural Stiffeners", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(rawName, "Rigidizadores estructurales", StringComparison.OrdinalIgnoreCase))
            {
                return "Structural Stiffeners";
            }

            return rawName;
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

        private bool TryReadLengthParameterAsMeters(Element element, string parameterName, out double value)
        {
            value = 0.0;

            if (element == null || string.IsNullOrWhiteSpace(parameterName))
                return false;

            Parameter p = element.LookupParameter(parameterName);
            if (p == null || !p.HasValue)
                return false;

            if (p.StorageType != StorageType.Double)
                return false;

            try
            {
                value = UnitUtils.ConvertFromInternalUnits(p.AsDouble(), UnitTypeId.Meters);
                return true;
            }
            catch
            {
                value = 0.0;
                return false;
            }
        }

        private bool TryReadVolumeParameterAsCubicMeters(Element element, string parameterName, out double value)
        {
            value = 0.0;

            if (element == null || string.IsNullOrWhiteSpace(parameterName))
                return false;

            Parameter p = element.LookupParameter(parameterName);
            if (p == null || !p.HasValue)
                return false;

            if (p.StorageType != StorageType.Double)
                return false;

            try
            {
                value = UnitUtils.ConvertFromInternalUnits(p.AsDouble(), UnitTypeId.CubicMeters);
                return true;
            }
            catch
            {
                value = 0.0;
                return false;
            }
        }
    }
}