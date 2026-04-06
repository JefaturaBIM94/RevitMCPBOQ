namespace NavisBOQ.Core.Models
{
    public class BoqSummaryRow
    {
        public string Nivel { get; set; } = "";
        public string Cat { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string TipoDesc { get; set; } = "";
        public string TipoMaterial { get; set; } = "";
        public double TipoAncho { get; set; }
        public double TipoEspesor { get; set; }

        public double Area { get; set; }
        public double Vol { get; set; }
        public double Long_ { get; set; }
        public double Cantidad { get; set; }
        public string Unidad { get; set; } = "";
        public int N { get; set; }

        public string UbicacionEstructural { get; set; } = "";
    }
}