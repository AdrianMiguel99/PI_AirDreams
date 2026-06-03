namespace AirDreams.API.DTOs
{
    public class UpdateFlightWeightDto
    {
        public string TransactionId { get; set; } = string.Empty;

        public decimal LuggageWeight { get; set; }

        public decimal CarryOnWeight { get; set; }
    }
}