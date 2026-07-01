using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IPurchaseService
    {
        Task<PaymentResponseDTO> ConfirmPurchaseAsync(ConfirmPurchaseDTO dto);
        Task<bool> CheckFlightAvailabilityAsync( string numberFlight, string seatClass, int requestedSeats);
    }
}