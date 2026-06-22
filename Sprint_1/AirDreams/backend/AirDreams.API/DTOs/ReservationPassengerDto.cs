namespace AirDreams.API.DTOs
{
    public class ReservationPassengerDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public int IdPassenger { get; set; }
        public string PassengerName { get; set; } = string.Empty;
    }
}
