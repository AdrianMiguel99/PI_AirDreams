namespace AirDreams.API.Repositories
{
    public interface ILuggageRepository
    {
        Task<string?> CreateLuggageAsync(string type, int quantity);
        Task<bool> RegisterLuggageAsync(int idPassenger, string transactionIdItinerary, string luggageNumber);
        Task<bool> CheckLuggageWeightAsync(string flightId, decimal luggageWeight);
        Task<bool> CheckCarryOnWeightAsync(string flightId, decimal carryOnWeight);
    }
}
