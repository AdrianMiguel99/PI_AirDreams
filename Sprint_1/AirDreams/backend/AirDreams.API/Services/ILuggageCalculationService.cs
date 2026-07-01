using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface ILuggageCalculationService
    {
        Task<LuggageTotalResponse> CalculateTotalsAsync(LuggageTotalRequestDTO request);
    }
}