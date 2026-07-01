namespace AirDreams.API.DTOs
{
    public class ReservationLuggageSegmentDto
    {
        public string FlightNumber { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public decimal CheckedPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal Multiplier { get; set; }
    }
}