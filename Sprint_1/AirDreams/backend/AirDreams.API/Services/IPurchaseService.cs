using AirDreams.API.Models.Dtos;
using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IPurchaseService
    {
        Task<PaymentResponseDto> ConfirmPurchaseAsync(ConfirmPurchaseDto dto);
        Task<bool> CheckFlightAvailabilityAsync( string numberFlight, string seatClass, int requestedSeats);
        Task<ExternalPaymentResponseDto> ConfirmExternalPurchaseAsync(ExternalOrderRequestDto dto);
    }
}