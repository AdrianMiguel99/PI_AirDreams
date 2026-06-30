namespace AirDreams.API.DTOs
{
    public class ExternalOrderRequestDto
    {
        public string flightGUID { get; set; } = "";
        public bool firstClass { get; set; }
        public List<ExternalPassengerDto> passengers { get; set; } = new();
        public ExternalBuyerDto buyer { get; set; } = new();
        public ExternalPaymentDto payment { get; set; } = new();
    }
}