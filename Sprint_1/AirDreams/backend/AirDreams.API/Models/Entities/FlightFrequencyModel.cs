namespace AirDreams.API.Models
{
    public class FlightFrequencyModel
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan EstimatedArrivalTime { get; set; }
        public DateTime EndingDate { get; set; }
        public bool Active { get; set; }
    }
}