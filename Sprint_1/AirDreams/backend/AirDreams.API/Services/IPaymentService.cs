using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto dto);
    }
}