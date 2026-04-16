namespace NavisBOQ.Core.Models
{
    public class SteelRunRow
    {
        public string Nivel { get; set; } = "Sin nivel";
        public string Categoria { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";

        public string SectionName { get; set; } = "";
        public string SectionShape { get; set; } = "";
        public string CodeName { get; set; } = "";

        public string MaterialEst { get; set; } = "";
        public string Mark { get; set; } = "";
        public string ElemId { get; set; } = "";

        public double NominalWeightKgm { get; set; }
        public double LinearWeightKgm { get; set; }
        public double LengthM { get; set; }
        public double VolumeM3 { get; set; }

        public double Weight2022Kg { get; set; }
        public double PesoKg { get; set; }

        public string Metodo { get; set; } = "N/D";
        public string Advertencia { get; set; } = "";
    }
}