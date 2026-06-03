using AirDreams.API.Models.Dtos;
using System.Data;

namespace AirDreams.API.Repositories
{
    public interface IPurchaseRepository
    {
        Task ConfirmPurchaseAsync(ConfirmPurchaseDto dto, string? cardLastFour);

        Task<bool> CheckFlightAvailabilityAsync(string numberFlight, string seatClass, int requestedSeats);

        Task ReserveFlightSeatsAsync( string numberFlight, string seatClass, int requestedSeats, IDbTransaction transaction);
    }
}