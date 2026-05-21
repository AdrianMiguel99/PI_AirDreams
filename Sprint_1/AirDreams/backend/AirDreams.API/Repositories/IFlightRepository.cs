using AirDreams.API.Models.Entities;

namespace AirDreams.API.Repositories
{
    public interface IFlightRepository
    {
        Task<string?> ValidateApiKeyAsync(string apiKey);

        Task<IEnumerable<dynamic>> SearchFlightsAsync(
            string origin,
            string destination,
            DateTime earliestTime,
            DateTime latestTime,
            int quantityOfPassengers
        );

        Task<IEnumerable<dynamic>> SearchOneStopFlightsAsync(
            string origin,
            string destination,
            DateTime earliestTime,
            DateTime latestTime,
            int quantityOfPassengers
        );

    }
}