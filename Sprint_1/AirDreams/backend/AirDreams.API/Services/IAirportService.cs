using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Services
{
    public interface IAirportService
    {
        Task<List<AirportDto>> GetAllAsync();
        Task<AirportDto?> GetByCodeAsync(string code);
        Task<AirportDto> CreateAsync(CreateAirportDto dto, byte adminId);
    }
}