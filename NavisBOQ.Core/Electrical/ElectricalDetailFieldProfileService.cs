using System.Collections.Generic;
using NavisBOQ.Core.Constants;

namespace NavisBOQ.Core.Electrical
{
    public class ElectricalDetailFieldProfileService
    {
        public List<PropertyFieldRequest> GetProfile(string runName, string profileName)
        {
            var fields = new List<PropertyFieldRequest>();
            var profile = (profileName ?? "").Trim().ToLowerInvariant();

            if (profile == DetailProfileNames.TubeFocus)
            {
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "TypeNodeName", OutputField = "TypeNodeName" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "CategoryDisplay", OutputField = "CategoryDisplay" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "LoadClassification", OutputField = "LoadClassification" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "Description", OutputField = "Description" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "KeynoteNote", OutputField = "KeynoteNote" });

                fields.Add(new PropertyFieldRequest { SourceNode = "instance", PropertyInternalName = "SizeText", OutputField = "SizeText" });
                fields.Add(new PropertyFieldRequest { SourceNode = "instance", PropertyInternalName = "LengthByInstanceMl", OutputField = "LengthByInstanceMl" });
                fields.Add(new PropertyFieldRequest { SourceNode = "custom", PropertyInternalName = "CustomPartida", OutputField = "CustomPartida" });

                return fields;
            }

            if (profile == DetailProfileNames.FixtureFocus)
            {
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "FamilyTypeName", OutputField = "FamilyTypeName" });
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "OmniClassTitle", OutputField = "OmniClassTitle" });
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "PieceType", OutputField = "PieceType" });
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "TypeComments", OutputField = "TypeComments" });
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "Description", OutputField = "Description" });
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "Url", OutputField = "Url" });

                fields.Add(new PropertyFieldRequest { SourceNode = "instance", PropertyInternalName = "PanelInstance", OutputField = "PanelInstance" });
                fields.Add(new PropertyFieldRequest { SourceNode = "custom", PropertyInternalName = "CustomPartida", OutputField = "CustomPartida" });

                return fields;
            }

            if (profile == DetailProfileNames.ElectricalFull)
            {
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "FamilyTypeName", OutputField = "FamilyTypeName" });
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "OmniClassTitle", OutputField = "OmniClassTitle" });
                fields.Add(new PropertyFieldRequest { SourceNode = "family", PropertyInternalName = "PieceType", OutputField = "PieceType" });

                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "TypeNodeName", OutputField = "TypeNodeName" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "CategoryDisplay", OutputField = "CategoryDisplay" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "DimensionA", OutputField = "DimensionA" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "DimensionB", OutputField = "DimensionB" });
                fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "Description", OutputField = "Description" });

                fields.Add(new PropertyFieldRequest { SourceNode = "instance", PropertyInternalName = "ElectricalData", OutputField = "ElectricalData" });
                fields.Add(new PropertyFieldRequest { SourceNode = "instance", PropertyInternalName = "PanelName", OutputField = "PanelName" });
                fields.Add(new PropertyFieldRequest { SourceNode = "instance", PropertyInternalName = "MainBreakerPower", OutputField = "MainBreakerPower" });
                fields.Add(new PropertyFieldRequest { SourceNode = "custom", PropertyInternalName = "CustomPartida", OutputField = "CustomPartida" });

                return fields;
            }

            fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "TypeNodeName", OutputField = "TypeNodeName" });
            fields.Add(new PropertyFieldRequest { SourceNode = "type", PropertyInternalName = "CategoryDisplay", OutputField = "CategoryDisplay" });

            return fields;
        }
    }
}