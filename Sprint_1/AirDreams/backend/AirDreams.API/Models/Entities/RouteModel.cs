namespace AirDreams.API.Models
{
    public class RouteModel
    {
        public int RouteID { get; set; }
        public string CodeAirportOrigin { get; set; } = string.Empty;
        public string CodeAirportDestination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal BasePriceTurist { get; set; }
        public decimal BasePriceFirstClass { get; set; }
        public TimeSpan FlightDuration { get; set; }
        public double FlightDistance { get; set; }
        public string AircraftModel { get; set; } = string.Empty;
        public int MaxWeightLuggage { get; set; }
        public decimal PriceLuggage { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}