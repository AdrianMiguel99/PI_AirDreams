using AirDreams.API.DTOs;

namespace AirDreams.API.Repositories
{
    public interface IReservationRepository
    {
        Task<ReservationDetailsDto?> GetReservationDetailsAsync(string reservationCode);
    }
}
