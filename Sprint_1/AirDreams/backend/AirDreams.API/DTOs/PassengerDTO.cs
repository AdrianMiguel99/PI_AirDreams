namespace AirDreams.API.DTOs
{
    public class PassengerDto
    {
        public int IdPassenger { get; set; }
        public string NamePassenger { get; set; } = string.Empty;
        public string LastnamesPassenger { get; set; } = string.Empty;
        public string EmailPassenger { get; set; } = string.Empty;
        public long? Telephone { get; set; }
        public string Country { get; set; } = string.Empty;
    }
}
