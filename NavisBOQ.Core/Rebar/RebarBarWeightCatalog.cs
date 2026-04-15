using System;
using System.Collections.Generic;
using System.Linq;

namespace NavisBOQ.Core.Rebar
{
    public sealed class RebarBarWeightInfo
    {
        public string BarNumber { get; set; } = "";
        public double DiameterMm { get; set; }
        public double LinearWeightKgm { get; set; }
    }

    public static class RebarBarWeightCatalog
    {
        private static readonly List<RebarBarWeightInfo> _items = new List<RebarBarWeightInfo>
        {
            new RebarBarWeightInfo { BarNumber = "#2.5", DiameterMm = 7.9,  LinearWeightKgm = 0.380 },
            new RebarBarWeightInfo { BarNumber = "#3",   DiameterMm = 9.5,  LinearWeightKgm = 0.560 },
            new RebarBarWeightInfo { BarNumber = "#4",   DiameterMm = 12.7, LinearWeightKgm = 0.994 },
            new RebarBarWeightInfo { BarNumber = "#5",   DiameterMm = 15.9, LinearWeightKgm = 1.552 },
            new RebarBarWeightInfo { BarNumber = "#6",   DiameterMm = 19.1, LinearWeightKgm = 2.235 },
            new RebarBarWeightInfo { BarNumber = "#8",   DiameterMm = 25.4, LinearWeightKgm = 3.975 }
        };

        public static bool TryResolveByDiameterMm(double diameterMm, out RebarBarWeightInfo info, double toleranceMm = 0.25)
        {
            info = _items
                .OrderBy(x => Math.Abs(x.DiameterMm - diameterMm))
                .FirstOrDefault(x => Math.Abs(x.DiameterMm - diameterMm) <= toleranceMm);

            return info != null;
        }
    }
}