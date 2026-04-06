using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavisBOQ.Revit.Plugin.Transport
{
    public class PluginHttpRequest
    {
        public string Tool { get; set; } = "";
        public string ParamsJson { get; set; } = "{}";
    }
}