namespace NavisBOQ.Core.Models
{
    public class RebarRunRow
    {
        public string Nivel { get; set; } = "Sin nivel";
        public string Categoria { get; set; } = "Rebar";
        public string Tipo { get; set; } = "";
        public string Shape { get; set; } = "";
        public string BarNumber { get; set; } = "";
        public double DiameterMm { get; set; }
        public double LinearWeightKgm { get; set; }

        // Longitud de una sola barra
        public double BarLengthM { get; set; }

        // Cantidad de barras del set
        public int Quantity { get; set; }

        // Longitud total del set = BarLengthM * Quantity
        public double TotalLengthM { get; set; }

        // Peso total del set
        public double TotalWeightKg { get; set; }

        public string ElemId { get; set; } = "";
    }
}