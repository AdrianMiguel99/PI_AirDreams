namespace AirDreams.API.DTOs
{
    public class ReservationLuggageDto
    {
        public int IdPassenger { get; set; }
        public string TransactionIdItinerary { get; set; } = string.Empty;
        public string LuggageNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public byte Quantity { get; set; }

        public string FlightNumber { get; set; } = string.Empty;
        public int RouteId { get; set; }
        public decimal CheckedPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal Multiplier { get; set; }
    }
}