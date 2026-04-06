using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavisBOQ.Revit.Plugin.Transport
{
    public class PluginHttpResponse
    {
        public bool Ok { get; set; }
        public object Data { get; set; }
        public string Error { get; set; } = "";
    }
}