using AirDreams.API.DTOs;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class LuggageCalculationService : ILuggageCalculationService
    {
        private readonly ILuggageCalculationRepository _repository;

        public LuggageCalculationService(ILuggageCalculationRepository repository)
        {
            _repository = repository;
        }

        public async Task<LuggageTotalResponse> CalculateTotalsAsync(LuggageTotalRequestDTO request)
        {
            decimal totalChecked = 0;
            decimal totalCarryOn = 0;

            foreach (var seg in request.Segments)
            {
                totalChecked += await _repository.CalculateAsync(
                    seg.CheckedPrice, seg.Multiplier, request.CheckedQuantity);
                totalCarryOn += await _repository.CalculateAsync(
                    seg.CarryOnPrice, seg.Multiplier, request.CarryOnQuantity);
            }

            return new LuggageTotalResponse
            {
                CheckedTotal = totalChecked,
                CarryOnTotal = totalCarryOn
            };
        }
    }
}