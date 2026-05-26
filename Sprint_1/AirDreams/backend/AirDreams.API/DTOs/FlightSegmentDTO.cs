namespace AirDreams.API.DTOs
{
    public class FlightSegmentDTO
    {
        public int RouteId { get; set; }
        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public AirportDTO DepartureAirport { get; set; }
        public AirportDTO ArrivalAirport { get; set; }
        public TimeSpan Duration { get; set; }
    }

}