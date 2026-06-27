namespace AirDreams.API.Repositories.Interfaces
{
    public interface ICancellationRepository
    {
        Task<bool> ItineraryExistsAsync(string transactionId);
        Task<bool> IsItineraryCancelledAsync(string transactionId);
        Task CancelItineraryAsync(string transactionId);
    }
}