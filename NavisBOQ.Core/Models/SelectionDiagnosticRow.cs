namespace NavisBOQ.Core.Models
{
    public class SelectionDiagnosticRow
    {
        public string ElementId { get; set; } = "";
        public string UniqueId { get; set; } = "";
        public string Category { get; set; } = "";
        public string CategoryId { get; set; } = "";
        public string Family { get; set; } = "";
        public string Type { get; set; } = "";
        public string Level { get; set; } = "";
        public string SystemName { get; set; } = "";
        public string SizeText { get; set; } = "";
        public double LengthM { get; set; }
    }
}