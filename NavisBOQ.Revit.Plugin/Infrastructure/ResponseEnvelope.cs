namespace NavisBOQ.Revit.Plugin.Infrastructure
{
    public class ResponseEnvelope
    {
        public bool Ok { get; set; }
        public string Message { get; set; } = "";
        public string DataJson { get; set; } = "";
        public string Error { get; set; } = "";
    }
}