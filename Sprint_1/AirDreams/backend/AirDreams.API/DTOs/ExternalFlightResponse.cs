namespace AirDreams.ExternalAPI.DTOs
{
    public class ExternalFlightResponse
    {
        public List<ExternalResponseFlightDTO> Flights { get; set; } = new();
    }

    public class ExternalResponseFlightDTO
    {
        public string flightGUID { get; set; } = string.Empty;
        public string departureTime { get; set; } = string.Empty;
        public string arrivalTime { get; set; } = string.Empty;
        public string duration { get; set; } = string.Empty;
        public ExternalResponseAirportDTO departureAirport { get; set; } = new();
        public ExternalResponseAirportDTO arrivalAirport { get; set; } = new();
        public decimal touristPrice { get; set; }
        public decimal firstClassPrice { get; set; }
        public decimal carryOnPrice { get; set; }
        public decimal checkedPrice { get; set; }
    }

    public class ExternalResponseAirportDTO
    {
        public string code { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
    }
}