using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavisBOQ.Core.Models
{
    public class SnapshotReadOptions
    {
        public bool IncludeIdentity { get; set; } = true;
        public bool IncludeGeometry { get; set; } = true;
        public bool IncludeTypeData { get; set; } = true;
        public bool IncludeSystemData { get; set; } = false;
        public bool IncludeElectricalData { get; set; } = false;
        public bool IncludeSteelData { get; set; } = false;
        public bool IncludeHvacData { get; set; } = false;
        public bool IncludeDiagnostics { get; set; } = true;

        public static SnapshotReadOptions ForCorrida1()
        {
            return new SnapshotReadOptions
            {
                IncludeIdentity = true,
                IncludeGeometry = true,
                IncludeTypeData = true,
                IncludeSystemData = false,
                IncludeElectricalData = false,
                IncludeSteelData = false,
                IncludeHvacData = false,
                IncludeDiagnostics = true
            };
        }

        public static SnapshotReadOptions ForCorrida4()
        {
            return new SnapshotReadOptions
            {
                IncludeIdentity = true,
                IncludeGeometry = true,
                IncludeTypeData = true,
                IncludeSystemData = true,
                IncludeElectricalData = true,
                IncludeSteelData = false,
                IncludeHvacData = false,
                IncludeDiagnostics = true
            };
        }

        public static SnapshotReadOptions ForCorrida5()
        {
            return new SnapshotReadOptions
            {
                IncludeIdentity = true,
                IncludeGeometry = true,
                IncludeTypeData = true,
                IncludeSystemData = true,
                IncludeElectricalData = false,
                IncludeSteelData = false,
                IncludeHvacData = true,
                IncludeDiagnostics = true
            };
        }
    }
}