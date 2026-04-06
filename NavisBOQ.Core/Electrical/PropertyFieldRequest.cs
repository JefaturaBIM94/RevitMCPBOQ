namespace NavisBOQ.Core.Electrical
{
    public class PropertyFieldRequest
    {
        public string SourceNode { get; set; } = "";
        public string CategoryInternalName { get; set; } = "";
        public string PropertyInternalName { get; set; } = "";
        public string OutputField { get; set; } = "";
        public bool Required { get; set; }
    }
}