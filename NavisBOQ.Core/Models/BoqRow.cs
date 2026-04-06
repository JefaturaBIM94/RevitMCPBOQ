namespace NavisBOQ.Core.Models
{
    public class BoqRow
    {
        public string Nivel { get; set; } = "";
        public string Categoria { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string TipoDesc { get; set; } = "";
        public string TipoMaterial { get; set; } = "";
        public double TipoAncho { get; set; }
        public double TipoEspesor { get; set; }

        public double Area { get; set; }
        public double Volumen { get; set; }
        public double Longitud { get; set; }

        public double Cantidad { get; set; }
        public string Unidad { get; set; } = "";

        public string ElemId { get; set; } = "";
        public string UbicacionEstructural { get; set; } = "";
    }
}