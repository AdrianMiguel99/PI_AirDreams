namespace AirDreams.API.DTOs
{
    public class ReservationLuggageDto
    {
        public int IdPassenger { get; set; }
        public string TransactionIdItinerary { get; set; } = string.Empty;
        public string LuggageNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public byte Quantity { get; set; }
    }
}