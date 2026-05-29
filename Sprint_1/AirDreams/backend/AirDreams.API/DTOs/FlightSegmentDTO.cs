namespace AirDreams.API.DTOs
{
    public class FlightSegmentDTO
    {
        public int RouteId { get; set; }
        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public AirportDTO DepartureAirport { get; set; } = new AirportDTO();
        public AirportDTO ArrivalAirport { get; set; } = new AirportDTO();
        public TimeSpan Duration { get; set; }
    }

}