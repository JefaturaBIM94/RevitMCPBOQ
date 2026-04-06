using System;

namespace NavisBOQ.Revit.Plugin.Infrastructure
{
    public class RequestEnvelope
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public string CommandName { get; set; } = "";
        public string PayloadJson { get; set; } = "";
    }
}