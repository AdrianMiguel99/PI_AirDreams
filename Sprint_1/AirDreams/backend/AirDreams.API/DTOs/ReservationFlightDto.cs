namespace AirDreams.API.DTOs
{
    public class ReservationFlightDto
    {
        public string TransactionId { get; set; } = string.Empty;

        public string ItineraryStatus { get; set; } = string.Empty;

        public string SeatClass { get; set; } = string.Empty;

        public string FlightNumber { get; set; } = string.Empty;

        public DateTime DepartureDate { get; set; }

        public string FlightState { get; set; } = string.Empty;

        public string OriginCode { get; set; } = string.Empty;
        public string OriginCity { get; set; } = string.Empty;
        public string OriginCountry { get; set; } = string.Empty;

        public string DestinationCode { get; set; } = string.Empty;
        public string DestinationCity { get; set; } = string.Empty;
        public string DestinationCountry { get; set; } = string.Empty;

        public string? AircraftModel { get; set; }

        public TimeSpan Duration { get; set; }

        public string AirlineName { get; set; } = string.Empty;

        public bool IsAirDreams { get; set; }
    }
}