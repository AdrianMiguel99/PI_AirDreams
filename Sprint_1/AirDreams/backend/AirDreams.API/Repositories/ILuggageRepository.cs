namespace AirDreams.API.Repositories
{
    public interface ILuggageRepository
    {
        Task<string?> CreateLuggageAsync(string type, int quantity);
        Task<bool> RegisterLuggageAsync(int idPassenger, string transactionIdItinerary, string luggageNumber);
    }
}
