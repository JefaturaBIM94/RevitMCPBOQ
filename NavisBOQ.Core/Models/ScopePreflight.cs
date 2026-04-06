using System.Collections.Generic;

namespace NavisBOQ.Core.Models
{
    public class ScopePreflight
    {
        public string ScopeResolved { get; set; } = "";
        public int VisitedNodes { get; set; }
        public int CandidateItems { get; set; }
        public int GeometricItems { get; set; }
        public int DistinctLevels { get; set; }
        public int DistinctCategories { get; set; }
        public string RiskBand { get; set; } = "green";
        public bool AllowRun { get; set; }
        public bool ForceSummary { get; set; }
        public string Message { get; set; } = "";
        public List<string> SuggestedSegmentation { get; set; } = new List<string>();
    }
}