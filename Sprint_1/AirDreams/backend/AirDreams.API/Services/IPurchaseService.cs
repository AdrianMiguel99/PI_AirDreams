using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Services
{
    public interface IPurchaseService
    {
        Task<PaymentResponseDto> ConfirmPurchaseAsync(ConfirmPurchaseDto dto);
    }
}