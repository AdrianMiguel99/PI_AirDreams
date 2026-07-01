using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> ProcessPaymentAsync(PaymentRequestDTO dto);
    }
}