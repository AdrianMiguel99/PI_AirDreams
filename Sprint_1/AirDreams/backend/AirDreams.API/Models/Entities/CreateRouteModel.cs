namespace AirDreams.API.Models
{
    public class CreateRouteModel
    {
        public int AdminID { get; set; }
        public string CodeAirportSalida { get; set; } = string.Empty;
        public string CodeAirportLlegada { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public decimal FirstClassPrice { get; set; }
        public decimal TuristClassPrice { get; set; }
        public TimeSpan StimatedTime { get; set; }
        public decimal Distance { get; set; }

        public List<FlightFrequencyModel> Frequencies { get; set; } = new();
    }
}