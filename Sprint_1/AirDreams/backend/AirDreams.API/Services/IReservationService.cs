using AirDreams.API.DTOs;
namespace AirDreams.API.Services
{
    public interface IReservationService
    {
        Task<ReservationDetailsDTO?> GetReservationDetailsAsync(string reservationCode);
    }
}
