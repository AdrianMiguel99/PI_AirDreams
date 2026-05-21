namespace AirDreams.ExternalAPI.DTOs
{
    public class ExternalFlightResponse
    {
        public List<FlightDTO> Flights { get; set; } = new();
    }

    public class FlightDTO
    {
        public string FlightGUID { get; set; } = string.Empty;
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public AirportDTO DepartureAirport { get; set; } = new();
        public AirportDTO ArrivalAirport { get; set; } = new();
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal CheckedPrice { get; set; }
    }

    public class AirportDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}