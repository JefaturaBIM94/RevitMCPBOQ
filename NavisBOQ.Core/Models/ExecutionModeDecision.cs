using System.Collections.Generic;

namespace NavisBOQ.Core.Models
{
    public class ExecutionModeDecision
    {
        public string Mode { get; set; } = "auto_safe";
        public string Reason { get; set; } = "";
        public bool ForceSummary { get; set; }
        public bool AllowAutoRun { get; set; } = true;
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> SuggestedActions { get; set; } = new List<string>();
    }
}