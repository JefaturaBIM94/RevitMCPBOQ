using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NavisBOQ.Core.HVAC
{
    public static class HvacSizeParser
    {
        private static readonly Regex RectInchRegex =
            new Regex(@"(?<a>\d+(?:\.\d+)?)\s*""?\s*x\s*(?<b>\d+(?:\.\d+)?)\s*""?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex SingleInchRegex =
            new Regex(@"(?<d>\d+(?:\.\d+)?)\s*""",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex MmRegex =
            new Regex(@"(?<a>\d+(?:\.\d+)?)\s*mm\s*x\s*(?<b>\d+(?:\.\d+)?)\s*mm",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex SingleMmRegex =
            new Regex(@"(?<d>\d+(?:\.\d+)?)\s*mm",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool TryParseSize(string raw, out double aM, out double bM, out double dM, out string shape)
        {
            aM = 0;
            bM = 0;
            dM = 0;
            shape = "Unknown";

            if (string.IsNullOrWhiteSpace(raw))
                return false;

            string s = raw.Trim();

            // 508 mm x 406 mm
            var mmRect = MmRegex.Match(s);
            if (mmRect.Success)
            {
                aM = MmToM(Parse(mmRect.Groups["a"].Value));
                bM = MmToM(Parse(mmRect.Groups["b"].Value));
                shape = "Rectangular";
                return true;
            }

            // 20"x14"  o  20"x14"-20"x14"
            var rect = RectInchRegex.Match(s);
            if (rect.Success)
            {
                aM = InchToM(Parse(rect.Groups["a"].Value));
                bM = InchToM(Parse(rect.Groups["b"].Value));
                shape = "Rectangular";
                return true;
            }

            // 14"
            var inchSingle = SingleInchRegex.Match(s);
            if (inchSingle.Success)
            {
                dM = InchToM(Parse(inchSingle.Groups["d"].Value));
                shape = "Circular";
                return true;
            }

            // 508 mm
            var mmSingle = SingleMmRegex.Match(s);
            if (mmSingle.Success)
            {
                dM = MmToM(Parse(mmSingle.Groups["d"].Value));
                shape = "Circular";
                return true;
            }

            return false;
        }

        public static double InchToM(double v) => v * 0.0254;
        public static double MmToM(double v) => v / 1000.0;

        private static double Parse(string s)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                return v;

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
                return v;

            return 0;
        }
    }
}
