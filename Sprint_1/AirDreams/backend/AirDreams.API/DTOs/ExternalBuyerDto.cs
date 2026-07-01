namespace AirDreams.API.DTOs
{
    public class ExternalBuyerDto
    {
        public string nationality { get; set; } = "";
        public string firstName { get; set; } = "";
        public string lastName { get; set; } = "";
        public string? lastName2 { get; set; }
        public string phoneNumber { get; set; } = "";
        public string email { get; set; } = "";
    }
}