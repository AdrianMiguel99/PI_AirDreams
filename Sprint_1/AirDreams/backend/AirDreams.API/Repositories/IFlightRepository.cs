using AirDreams.API.Models.Entities;

namespace AirDreams.API.Repositories
{
    public interface IFlightRepository
    {
        Task<List<Flight>> GetAllAsync();
        Task<string?> ValidateApiKeyAsync(string apiKey);
    }
}