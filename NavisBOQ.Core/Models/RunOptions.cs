namespace NavisBOQ.Core.Models
{
    public class RunOptions
    {
        public string ScopeMode { get; set; } = "all";
        public string SelectionSet { get; set; } = "";
        public string Level { get; set; } = "";
        public string OutputMode { get; set; } = "auto";
        public int MaxItems { get; set; } = 12000;
        public int MaxNodes { get; set; } = 50000;
        public bool StrictLimits { get; set; } = true;

        public string FilterCategory { get; set; }
        public string FilterType { get; set; }
    }
}