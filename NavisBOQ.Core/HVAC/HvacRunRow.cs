namespace NavisBOQ.Core.HVAC
{
    public class HvacRunRow
    {
        public string Nivel { get; set; } = "Sin nivel";
        public string SistemaClasificacion { get; set; } = "Sin sistema HVAC";
        public string NombreSistema { get; set; } = "Sin sistema HVAC";
        public string TipoSistema { get; set; } = "";

        public string CategoriaBoq { get; set; } = "";
        public string CategoriaRevit { get; set; } = "";
        public string FittingSubcategory { get; set; } = "";

        public string Familia { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string ElemId { get; set; } = "";

        public string Material { get; set; } = "";
        public string SegmentoTuberia { get; set; } = "";
        public string SizeText { get; set; } = "";
        public string Shape { get; set; } = "";

        public double WidthM { get; set; }
        public double HeightM { get; set; }
        public double DiameterM { get; set; }
        public double LengthM { get; set; }
        public double AreaM2 { get; set; }

        public string PressureClass { get; set; } = "";
        public string Gauge { get; set; } = "";
        public string GaugeCode { get; set; } = "";
        public double BaseInches { get; set; }

        public double PerimetroM { get; set; }
        public double LongitudLaminaM { get; set; }
        public double DimensionCM { get; set; }

        public double ThicknessMm { get; set; }
        public double DensityKgM3 { get; set; }

        public double Kg { get; set; }
        public double KgCalculated { get; set; }
        public double KgRevitParameter { get; set; }
        public bool HasKgRevitParameter { get; set; }

        public string KgMethod { get; set; } = "";

        public double Cantidad { get; set; }
        public string Unidad { get; set; } = "pza";
        public int NumTramos { get; set; }
    }
}