using System.Collections.Generic;
using NavisBOQ.Core.Models;

namespace NavisBOQ.Core.Electrical
{
    public class DetailExpansionRequest
    {
        public string RunName { get; set; } = "";
        public RunOptions Options { get; set; } = new RunOptions();

        public string DetailProfile { get; set; } = "type_light";
        public int MaxItems { get; set; } = 250;

        public bool IncludeFamilyFields { get; set; } = true;
        public bool IncludeTypeFields { get; set; } = true;
        public bool IncludeInstanceFields { get; set; } = true;

        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Types { get; set; } = new List<string>();
    }
}