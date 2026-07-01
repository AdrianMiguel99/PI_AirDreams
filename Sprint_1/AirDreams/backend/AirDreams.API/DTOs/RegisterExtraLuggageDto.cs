namespace AirDreams.API.DTOs
{
    public class RegisterExtraLuggageDto
    {
        public int IdPassenger { get; set; }

        public string TransactionIdItinerary { get; set; } = string.Empty;

        public List<ExtraLuggageItemDto> LuggageItems { get; set; } = new();
    }

    public class ExtraLuggageItemDto
    {
        public string Type { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}