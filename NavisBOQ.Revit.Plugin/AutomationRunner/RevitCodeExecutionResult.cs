using System.Collections.Generic;

namespace NavisBOQ.Revit.Plugin.AutomationRunner
{
    public class RevitCodeExecutionResult
    {
        public bool Ok { get; set; }
        public string Mode { get; set; } = "";
        public bool Executed { get; set; }
        public bool TransactionStarted { get; set; }
        public bool TransactionCommitted { get; set; }
        public string Message { get; set; } = "";
        public string OutputJson { get; set; } = "";
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> CompilerErrors { get; set; } = new List<string>();
    }
}