using AirDreams.API.DTOs;
namespace AirDreams.API.Services
{
    public interface IReservationService
    {
        Task<ReservationDetailsDto?> GetReservationDetailsAsync(string reservationCode);
    }
}
