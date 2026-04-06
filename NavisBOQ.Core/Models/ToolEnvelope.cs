using System.Collections.Generic;

namespace NavisBOQ.Core.Models
{
    public class ToolEnvelope<T>
    {
        public bool Ok { get; set; }
        public string Tool { get; set; } = "";
        public string ScopeMode { get; set; } = "";
        public string OutputMode { get; set; } = "";
        public ScopePreflight Preflight { get; set; }
        public T Data { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public string UserMessage { get; set; } = "";
    }
}