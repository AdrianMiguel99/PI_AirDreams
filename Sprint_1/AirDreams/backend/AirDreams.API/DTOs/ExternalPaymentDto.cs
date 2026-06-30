namespace AirDreams.API.DTOs
{
    public class ExternalPaymentDto
    {
        public string cardNumber { get; set; } = "";
        public string cardExpiration { get; set; } = "";
        public string cvv { get; set; } = "";
        public string cardHolderName { get; set; } = "";
    }
}