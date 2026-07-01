using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IAirportService
    {
        Task<List<AirportDTO>> GetAllAsync();
        Task<AirportDTO?> GetByCodeAsync(string code);
        Task<AirportDTO> CreateAsync(CreateAirportDTO dto, byte adminId);
        Task UpdateAsync(string code, UpdateAirportDTO dto);
        Task<(bool success, string message)> DeleteAsync(string code);
    }
}