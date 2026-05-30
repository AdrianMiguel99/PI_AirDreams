namespace AirDreams.API.Repositories
{
    public interface IPaymentRepository
    {
        Task<bool> UpdatePaymentAsync(string transactionId, string paymentMethod, string? cardLastFour,
            string buyerName, string buyerEmail, string buyerPhone);
    }
}