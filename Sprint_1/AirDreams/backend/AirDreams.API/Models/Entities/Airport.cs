namespace AirDreams.API.Models.Entities
{
    public class Airport
    {
        public string CodeAirport { get; set; } = string.Empty;  
        public byte AdminID { get; set; }
        public Admin Admin { get; set; } = null!;
        public string NameAirport { get; set; } = string.Empty; 
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? TimeZone { get; set; }
    }
}