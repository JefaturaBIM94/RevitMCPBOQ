namespace NavisBOQ.Core.HVAC
{
    public class HvacSummaryRow
    {
        public string Nivel { get; set; } = "Sin nivel";
        public string SistemaClasificacion { get; set; } = "Sin sistema HVAC";
        public string NombreSistema { get; set; } = "Sin sistema HVAC";
        public string CategoriaBoq { get; set; } = "";
        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";

        public string SizeText { get; set; } = "";
        public string Material { get; set; } = "";
        public string Gauge { get; set; } = "";
        public string Unidad { get; set; } = "pza";

        public int NumElementos { get; set; }
        public int NumTramos { get; set; }
        public double CantidadTotal { get; set; }
        public double LongitudTotalMl { get; set; }
        public double AreaTotalM2 { get; set; }
        public double KgTotal { get; set; }
    }
}
