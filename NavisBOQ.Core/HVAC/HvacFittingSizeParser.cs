using System.Globalization;
using System.Text.RegularExpressions;

namespace NavisBOQ.Core.HVAC
{
    public class HvacFittingSizeParseResult
    {
        public bool Success { get; set; }
        public bool IsRectangular { get; set; }
        public bool IsCircular { get; set; }

        public double Width1M { get; set; }
        public double Height1M { get; set; }
        public double Width2M { get; set; }
        public double Height2M { get; set; }

        public double Diameter1M { get; set; }
        public double Diameter2M { get; set; }

        public string Raw { get; set; } = "";
    }

    public static class HvacFittingSizeParser
    {
        private static readonly Regex RectToRectRegex =
            new Regex(
                @"(?<w1>\d+(?:\.\d+)?)\s*""?\s*x\s*(?<h1>\d+(?:\.\d+)?)\s*""?\s*-\s*(?<w2>\d+(?:\.\d+)?)\s*""?\s*x\s*(?<h2>\d+(?:\.\d+)?)\s*""?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex RectSingleRegex =
            new Regex(
                @"(?<w>\d+(?:\.\d+)?)\s*""?\s*x\s*(?<h>\d+(?:\.\d+)?)\s*""?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex CircularToCircularRegex =
            new Regex(
                @"(?<d1>\d+(?:\.\d+)?)\s*""?\s*-\s*(?<d2>\d+(?:\.\d+)?)\s*""?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex CircularSingleRegex =
            new Regex(
                @"(?<d>\d+(?:\.\d+)?)\s*""?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static HvacFittingSizeParseResult Parse(string raw)
        {
            var result = new HvacFittingSizeParseResult
            {
                Raw = raw ?? ""
            };

            if (string.IsNullOrWhiteSpace(raw))
                return result;

            string s = raw.Trim();

            var r2r = RectToRectRegex.Match(s);
            if (r2r.Success)
            {
                result.Success = true;
                result.IsRectangular = true;

                result.Width1M = InchToM(ParseDouble(r2r.Groups["w1"].Value));
                result.Height1M = InchToM(ParseDouble(r2r.Groups["h1"].Value));
                result.Width2M = InchToM(ParseDouble(r2r.Groups["w2"].Value));
                result.Height2M = InchToM(ParseDouble(r2r.Groups["h2"].Value));
                return result;
            }

            var rs = RectSingleRegex.Match(s);
            if (rs.Success)
            {
                result.Success = true;
                result.IsRectangular = true;

                result.Width1M = InchToM(ParseDouble(rs.Groups["w"].Value));
                result.Height1M = InchToM(ParseDouble(rs.Groups["h"].Value));
                result.Width2M = result.Width1M;
                result.Height2M = result.Height1M;
                return result;
            }

            var c2c = CircularToCircularRegex.Match(s);
            if (c2c.Success)
            {
                result.Success = true;
                result.IsCircular = true;

                result.Diameter1M = InchToM(ParseDouble(c2c.Groups["d1"].Value));
                result.Diameter2M = InchToM(ParseDouble(c2c.Groups["d2"].Value));
                return result;
            }

            var cs = CircularSingleRegex.Match(s);
            if (cs.Success)
            {
                result.Success = true;
                result.IsCircular = true;

                result.Diameter1M = InchToM(ParseDouble(cs.Groups["d"].Value));
                result.Diameter2M = result.Diameter1M;
                return result;
            }

            return result;
        }

        private static double ParseDouble(string s)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                return v;

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
                return v;

            return 0.0;
        }

        private static double InchToM(double valueInches)
        {
            return valueInches * 0.0254;
        }
    }
}
