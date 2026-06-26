using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.DTOs;
using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Models.Dtos
{
    public class ConnectedFlightDto
    {
        public FlightSegmentDTO InternalFlight { get; set; } = new();
        public ExternalResponseFlightDTO ExternalFlight { get; set; } = new();
        public double LayoverHours { get; set; }
        public decimal TouristPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
    }
}