namespace AirDreams.API.DTOs

{
    public class AirportDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public String? TimeZone { get; set; }
        public bool isActive { get; set; } = true;
    }
}