namespace NavisBOQ.Core.FPS
{
    public class FpsRunRow
    {
        public string Nivel { get; set; } = "Sin nivel";
        public string SistemaClasificacion { get; set; } = "Sin sistema FPS";
        public string NombreSistema { get; set; } = "Sin sistema FPS";
        public string TipoSistema { get; set; } = "";

        public string CategoriaBoq { get; set; } = "";
        public string CategoriaRevit { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string ElemId { get; set; } = "";

        public string Material { get; set; } = "";
        public string SegmentoTuberia { get; set; } = "";
        public string SizeText { get; set; } = "";

        public double DiameterM { get; set; }
        public double LengthM { get; set; }
        public double Cantidad { get; set; }

        public string Unidad { get; set; } = "pza";
        public int NumTramos { get; set; }
    }
}