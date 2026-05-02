namespace AirDreams.API.Models.Dtos
{
    public class AirportDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? TimeZone { get; set; }
    }
}