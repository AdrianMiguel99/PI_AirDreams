namespace AirDreams.API.DTOs
{
    public class LuggageMappingDTO
    {
        public int PassengerIndex { get; set; }
        public string LuggageNumber {get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public byte Quantity { get; set; }
    }
}
