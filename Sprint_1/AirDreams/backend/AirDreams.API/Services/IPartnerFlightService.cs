using AirDreams.ExternalAPI.DTOs;

namespace AirDreams.API.Services.Interfaces
{
    public interface IPartnerFlightService
    {
        Task<List<ExternalResponseFlightDTO>> SearchPartnerFlightsAsync(
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        );
        Task<ExternalResponseFlightDTO?> GetCachedFlightAsync(string flightGuid);
    }
}