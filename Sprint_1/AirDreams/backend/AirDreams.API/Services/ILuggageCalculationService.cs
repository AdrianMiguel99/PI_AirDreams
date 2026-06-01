using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Services
{
    public interface ILuggageCalculationService
    {
        Task<LuggageTotalResponse> CalculateTotalsAsync(LuggageTotalRequest request);
    }
}