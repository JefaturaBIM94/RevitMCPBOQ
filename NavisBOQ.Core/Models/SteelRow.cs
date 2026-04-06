namespace NavisBOQ.Core.Models
{
    public class SteelRow
    {
        public string Nivel { get; set; } = "";
        public string Categoria { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string SectionName { get; set; } = "";
        public string SectionShape { get; set; } = "";
        public string CodeName { get; set; } = "";
        public string MaterialEst { get; set; } = "";
        public string Metodo { get; set; } = "";
        public string ElemId { get; set; } = "";
        public string Mark { get; set; } = "";

        public double NominalWeight { get; set; }
        public double Length { get; set; }
        public double Volume { get; set; }
        public double PesoKg { get; set; }
    }
}