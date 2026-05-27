using AirDreams.API.DTOs;

namespace AirDreams.API.Services.Interfaces
{
    public interface IFlightService
    {
        Task<List<FlightItineraryDTO>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        );
    }
}
