using AirDreams.API.Models.Entities;

namespace AirDreams.API.Repositories
{
    public interface IAirportRepository
    {
        Task<List<Airport>> GetAllAsync();
        Task<Airport?> GetByCodeAsync(string code);
        Task<Airport> CreateAsync(Airport airport);
        Task<bool> ExistsAsync(string code);
        Task<bool> UpdateAsync(string code, string newName);
    }
}