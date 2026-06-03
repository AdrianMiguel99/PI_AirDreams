using AirDreams.API.Models;

namespace AirDreams.API.Services.Interfaces
{
    public interface ILuggageService
    {
        Task<(bool success, string message)> RegisterLuggageAsync(LuggageRegistrationModel model);
        Task<(bool success, string message)> CheckAvailabilityAsync(string flightId, int routeId, decimal luggageWeight, decimal carryOnWeight);
        Task<(bool success, string message)> UpdateFlightWeightAsync(string transactionId, decimal luggageWeight, decimal carryOnWeight);
    }
}
