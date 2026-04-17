namespace NavisBOQ.Core.FPS
{
    public class FpsSummaryRow
    {
        public string Nivel { get; set; } = "Sin nivel";
        public string SistemaClasificacion { get; set; } = "Sin sistema FPS";
        public string NombreSistema { get; set; } = "Sin sistema FPS";

        public string CategoriaBoq { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Material { get; set; } = "";
        public string SizeText { get; set; } = "";
        public string Unidad { get; set; } = "pza";

        public int NumElementos { get; set; }
        public int NumTramos { get; set; }
        public double CantidadTotal { get; set; }
        public double LongitudTotalMl { get; set; }
    }
}