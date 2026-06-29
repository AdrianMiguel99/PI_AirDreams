using AirDreams.API.DTOs;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<ReservationDetailsDto?> GetReservationDetailsAsync(string reservationCode)
    {
        if (string.IsNullOrWhiteSpace(reservationCode))
        {
            return null;
        }

        var reservationDetails = await _reservationRepository.GetReservationDetailsAsync(reservationCode);

        return reservationDetails;
    }
}
