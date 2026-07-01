using AirDreams.API.DTOs;

namespace AirDreams.API.Repositories
{
    public interface IPurchaseRepository
    {
        Task ConfirmPurchaseAsync(ConfirmPurchaseDTO dto, string? cardLastFour);
        Task<bool> CheckFlightAvailabilityAsync(string numberFlight, string seatClass, int requestedSeats);
        Task<Dictionary<string, decimal>> GetMultipliersByFlightsAsync(IEnumerable<string> flightNumbers);
    }
}