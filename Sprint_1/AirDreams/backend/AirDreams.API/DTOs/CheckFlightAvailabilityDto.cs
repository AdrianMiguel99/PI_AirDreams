namespace AirDreams.API.DTOs
{
    public class CheckFlightAvailabilityDto
    {
        public string NumberFlight { get; set; } = string.Empty;
        public string SeatClass { get; set; } = string.Empty;
        public int RequestedSeats { get; set; }
    }
}
