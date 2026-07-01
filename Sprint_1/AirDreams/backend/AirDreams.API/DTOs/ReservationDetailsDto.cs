namespace AirDreams.API.DTOs
{
    public class ReservationDetailsDTO
    {
        public List<ReservationPassengerDTO> Passengers { get; set; } = new();
        public List<ReservationFlightDTO> Flights { get; set; } = new();
    }
}
