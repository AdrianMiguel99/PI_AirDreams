using AirDreams.API.Models.Dtos;
using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto dto)
        {
            string? lastFour = null;
            if (dto.PaymentMethod.Equals("Card", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(dto.CardNumber))
            {
                var digits = new string(dto.CardNumber.Where(char.IsDigit).ToArray());
                lastFour = digits.Length >= 4 ? digits[^4..] : digits;
            }

            var success = await _paymentRepository.UpdatePaymentAsync(
                dto.TransactionId,
                dto.PaymentMethod,
                lastFour,
                dto.BuyerName,
                dto.BuyerEmail,
                dto.BuyerPhone
            );

            if (!success)
                throw new Exception("No se encontró el itinerario o no se pudo actualizar el pago.");

            return new PaymentResponseDto
            {
                Success = true,
                Message = "Pago procesado correctamente"
            };
        }
    }
}