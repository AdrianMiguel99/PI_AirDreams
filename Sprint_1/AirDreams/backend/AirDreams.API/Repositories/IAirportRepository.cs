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
        Task<bool> IsAirportInUseAsync(string code);
        Task<bool> SoftDeleteAsync(string code);
        Task<bool> HardDeleteAsync(string code);
        Task<(bool success, string message)> DeleteAirportAsync(string code, bool inUse);
    }
}