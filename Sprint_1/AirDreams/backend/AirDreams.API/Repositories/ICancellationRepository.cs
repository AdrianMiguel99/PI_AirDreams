namespace AirDreams.API.Repositories.Interfaces
{
    public interface ICancellationRepository
    {
        Task<bool> ItineraryExistsAsync(string transactionId);
        Task<bool> IsItineraryCancelledAsync(string transactionId);
        Task CancelItineraryAsync(string transactionId);

        Task<string?> GetBuyerEmailByTransactionIdAsync(string transactionId);

        Task SaveCancellationTokenAsync(
            string transactionId,
            string token,
            DateTime expirationDate
        );

        Task<(string transactionId, bool isUsed, DateTime expirationDate)?>
            GetCancellationTokenAsync(string token);

        Task MarkCancellationTokenAsUsedAsync(string token);
    }
}