namespace AirDreams.API.DTOs
{
    public class ReservationDetailsDto
    {
        public List<ReservationPassengerDto> Passengers { get; set; } = new();
        public List<ReservationFlightDto> Flights { get; set; } = new();
    }
}
