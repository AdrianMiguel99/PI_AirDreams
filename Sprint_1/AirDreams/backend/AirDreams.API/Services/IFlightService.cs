using AirDreams.API.DTOs;
using AirDreams.ExternalAPI.DTOs;

namespace AirDreams.API.Services.Interfaces
{
    public interface IFlightService
    {
        Task<List<FlightItineraryDTO>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers,
            bool includeStops
        );

        Task<List<ExternalResponseFlightDTO>> SearchFlightsByDestinationAsync(
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
            
        );
    }
}
