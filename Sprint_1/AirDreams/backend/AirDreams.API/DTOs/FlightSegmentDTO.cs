namespace AirDreams.API.DTOs
{
    public class FlightSegmentDTO
    {
        public string FlightNumber { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public decimal CarryOnPrice { get; set; }
        public decimal CheckedPrice { get; set; }
        public decimal Multiplier { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
        public AirportDTO DepartureAirport { get; set; } = new AirportDTO();
        public AirportDTO ArrivalAirport { get; set; } = new AirportDTO();
        public TimeSpan Duration { get; set; }
    }

}
