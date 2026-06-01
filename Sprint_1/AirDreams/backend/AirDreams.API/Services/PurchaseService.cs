using AirDreams.API.Models.Dtos;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _repository;

        public PurchaseService(IPurchaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaymentResponseDto> ConfirmPurchaseAsync(ConfirmPurchaseDto dto)
        {
            var amount = dto.PassengerCount * dto.PricePerPassenger
                        + (dto.Luggage?.Sum(l => l.LuggageItems.Sum(i => i.Subtotal)) ?? 0);

            string? lastFour = null;
            if (dto.PaymentMethod.Equals("Card", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(dto.CardNumber))
            {
                var digits = new string(dto.CardNumber.Where(char.IsDigit).ToArray());
                lastFour = digits.Length >= 4 ? digits[^4..] : null;
            }

            await _repository.ConfirmPurchaseAsync(dto, amount, lastFour);

            return new PaymentResponseDto { Success = true, Message = "Compra confirmada y pago procesado correctamente." };
        }
    }
}