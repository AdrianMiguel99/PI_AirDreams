namespace AirDreams.ExternalAPI.Dtos
{
    public class ExternalPassengerDto
    {
        public bool carryOn { get; set; }
        public int Checked { get; set; }
        public string passport { get; set; } = "";
        public DateTime passportExpirationDate { get; set; }
        public string passportCountry { get; set; } = "";
        public string firstName { get; set; } = "";
        public string lastName { get; set; } = "";
        public string? lastName2 { get; set; }
        public string gender { get; set; } = "";
        public DateTime birthDate { get; set; }
    }
}