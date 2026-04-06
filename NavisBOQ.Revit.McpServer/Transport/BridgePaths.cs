using System.IO;

namespace NavisBOQ.Revit.McpServer.Transport
{
    public static class BridgePaths
    {
        public static readonly string Root =
            @"C:\Users\fabian.banuet\source\repos\RevitMCPBOQ\.bridge";

        public static readonly string RequestFile = Path.Combine(Root, "request.json");
        public static readonly string ResponseFile = Path.Combine(Root, "response.json");
    }
}