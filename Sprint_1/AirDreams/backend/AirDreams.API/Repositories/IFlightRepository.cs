using AirDreams.API.Models.Entities;

namespace AirDreams.API.Repositories
{
    public interface IFlightRepository
    {
        Task<string?> ValidateApiKeyAsync(string apiKey);

        Task<IEnumerable<dynamic>> SearchFlightsAsync(
            string origin,
            string destination,
            TimeSpan earliestTime,
            TimeSpan latestTime,
            int quantityOfPassengers
        );

    }
}