namespace NavisBOQ.Core.Models
{
    public class SteelSummaryRow
    {
        public string Nivel { get; set; } = "";
        public string Categoria { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string SectionName { get; set; } = "";
        public string SectionShape { get; set; } = "";
        public string CodeName { get; set; } = "";
        public string Metodo { get; set; } = "";
        public string Advertencia { get; set; } = "";

        public double NominalWeight { get; set; }
        public int NumPiezas { get; set; }
        public double LengthTotal { get; set; }
        public double VolumeTotal { get; set; }
        public double PesoKg { get; set; }
        public double? PesoTonRef { get; set; }
    }
}
