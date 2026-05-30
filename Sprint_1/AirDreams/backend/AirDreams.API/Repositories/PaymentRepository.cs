using System.Data;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbConnection _connection;

        public PaymentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> UpdatePaymentAsync(string transactionId, string paymentMethod, string? cardLastFour,
            string buyerName, string buyerEmail, string buyerPhone)
        {
            var sql = @"
                UPDATE Itinerary
                SET paymentMethod = @PaymentMethod,
                    cardLastFour = @CardLastFour,
                    buyerName = @BuyerName,
                    buyerEmail = @BuyerEmail,
                    buyerPhone = @BuyerPhone,
                    paymentDate = GETDATE()
                WHERE transactionId = @TransactionId";

            var affected = await _connection.ExecuteAsync(sql, new
            {
                TransactionId = transactionId,
                PaymentMethod = paymentMethod,
                CardLastFour = cardLastFour,
                BuyerName = buyerName,
                BuyerEmail = buyerEmail,
                BuyerPhone = buyerPhone
            });

            return affected > 0;
        }
    }
}