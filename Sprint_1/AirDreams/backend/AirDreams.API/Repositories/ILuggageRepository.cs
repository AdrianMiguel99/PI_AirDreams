using AirDreams.API.DTOs;

namespace AirDreams.API.Repositories
{
    public interface ILuggageRepository
    {
        Task<string?> CreateLuggageAsync(string type, int quantity);
        Task<bool> RegisterLuggageAsync(int idPassenger, string transactionIdItinerary, string luggageNumber);
        Task<bool> CheckLuggageWeightAsync(string flightId, int routeId, decimal luggageWeight);
        Task<bool> CheckCarryOnWeightAsync(string flightId, int routeId, decimal carryOnWeight);
        Task<bool> UpdateFlightWeightAsync(string transactionId, decimal luggageWeight, decimal carryOnWeight);
        Task<ReservationLuggageResponseDto> GetReservationLuggageAsync(string transactionId);

        Task<bool> RegisterExtraLuggageAsync(int idPassenger, string transactionIdItinerary, string type, int quantity);
    }
}
