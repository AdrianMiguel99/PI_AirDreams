namespace AirDreams.API.DTOs
{
    public class RouteDTO
    {
        public int Id { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string Duration { get; set; }
        public AirportDTO DepartureAirport { get; set; }
        public AirportDTO ArrivalAirport { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
    }
}