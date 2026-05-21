namespace NavisBOQ.Revit.Plugin.AutomationRunner
{
    public class RevitCodeExecutionRequest
    {
        public string Code { get; set; } = "";
        public string Mode { get; set; } = "validate"; // validate | dry_run | execute
        public bool Confirmed { get; set; }
        public bool AllowModifications { get; set; }
        public bool UseTransaction { get; set; } = true;
        public string TransactionName { get; set; } = "NavisBOQ MCP Automation";
        public int TimeoutMs { get; set; } = 30000;
        public string ArgumentsJson { get; set; } = "{}";
    }
}
