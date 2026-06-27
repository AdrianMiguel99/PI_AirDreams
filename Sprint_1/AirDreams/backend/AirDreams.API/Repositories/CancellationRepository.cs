using System.Data;
using Dapper;
using AirDreams.API.Repositories.Interfaces;

namespace AirDreams.API.Repositories
{
    public class CancellationRepository : ICancellationRepository
    {
        private readonly IDbConnection _connection;

        public CancellationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> ItineraryExistsAsync(string transactionId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM Itinerary
                WHERE transactionId = @TransactionId;
            ";

            return await _connection.ExecuteScalarAsync<bool>(
                sql,
                new { TransactionId = transactionId }
            );
        }

        public async Task<bool> IsItineraryCancelledAsync(string transactionId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM Itinerary
                WHERE transactionId = @TransactionId
                    AND itineraryStatus = 'Cancelled';
            ";

            return await _connection.ExecuteScalarAsync<bool>(
                sql,
                new { TransactionId = transactionId }
            );
        }

        public async Task CancelItineraryAsync(string transactionId)
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            using var dbTransaction = _connection.BeginTransaction();

            try
            {
                const string cancelItinerary = @"
                    UPDATE Itinerary
                    SET itineraryStatus = 'Cancelled'
                    WHERE transactionId = @TransactionId;
                ";

                await _connection.ExecuteAsync(
                    cancelItinerary,
                    new { TransactionId = transactionId },
                    dbTransaction
                );

                dbTransaction.Commit();
            }
            catch
            {
                dbTransaction.Rollback();
                throw;
            }
        }
    }
}