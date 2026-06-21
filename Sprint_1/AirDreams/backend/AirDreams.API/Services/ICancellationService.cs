namespace AirDreams.API.Services.Interfaces
{
    public interface ICancellationService
    {
        Task<bool> CancelReservationAsync(string transactionId);
    }
}