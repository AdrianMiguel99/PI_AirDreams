namespace AirDreams.API.DTOs
{
    public class IncomeReportDTO
    {
        public string Mes { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int MesNumero { get; set; }
        public int CantidadVuelos { get; set; }
        public int TotalPasajerosPrimeraClase { get; set; }
        public int TotalPasajerosClaseEconomica { get; set; }
        public int TotalPasajeros { get; set; }
        public decimal IngresosTiquetes { get; set; }
        public int TotalMaletasDocumentadas { get; set; }
        public int TotalMaletasCarryOn { get; set; }
        public int TotalMaletas { get; set; }
        public decimal IngresosMaletas { get; set; }
        public decimal TotalIngresos { get; set; }
    }

    public class IncomeReportFiltersDTO
    {
        public IEnumerable<string> Origins { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> Destinations { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<int> Years { get; set; } = Enumerable.Empty<int>();
        public IEnumerable<string> Airlines { get; set; } = Enumerable.Empty<string>();
    }
}
