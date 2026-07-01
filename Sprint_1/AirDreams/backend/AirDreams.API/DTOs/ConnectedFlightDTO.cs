using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.DTOs;


namespace AirDreams.API.DTOs

{
    public class ConnectedFlightDTO
    {
        public FlightSegmentDTO InternalFlight { get; set; } = new();
        public ExternalResponseFlightDTO ExternalFlight { get; set; } = new();
        public double LayoverHours { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
    }
}