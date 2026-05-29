using System.Text.Json;

namespace AirDreams.ExternalAPI.Services
{
    public interface IFlightService
    {
        Task<(int StatusCode, JsonElement Content)> SearchFlightAsync(string destination, DateTime earliestDeparture, DateTime latestDeparture, int quantityOfPassengers);
    }
}
