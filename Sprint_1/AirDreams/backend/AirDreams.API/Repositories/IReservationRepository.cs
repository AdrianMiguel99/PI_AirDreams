using AirDreams.API.DTOs;

namespace AirDreams.API.Repositories
{
    public interface IReservationRepository
    {
        Task<ReservationDetailsDTO?> GetReservationDetailsAsync(string reservationCode);
    }
}
