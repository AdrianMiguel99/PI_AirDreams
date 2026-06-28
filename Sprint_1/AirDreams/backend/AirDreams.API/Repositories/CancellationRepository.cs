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
                const string getReservationData = @"
                    SELECT
                        seatClass AS SeatClass,
                        (
                            SELECT COUNT(*)
                            FROM PassengerItinerary
                            WHERE transactionId = @TransactionId
                        ) AS PassengerCount
                    FROM Itinerary
                    WHERE transactionId = @TransactionId;
                ";

                var reservation = await _connection.QuerySingleAsync<dynamic>(
                    getReservationData,
                    new { TransactionId = transactionId },
                    dbTransaction
                );

                string seatClass = reservation.SeatClass;
                int passengerCount = reservation.PassengerCount;

                if (seatClass == "FirstClass")
                {
                    const string releaseFirstClassSeats = @"
                        UPDATE f
                        SET occupiedFirstclass =
                            CASE
                                WHEN occupiedFirstclass - @PassengerCount < 0 THEN 0
                                ELSE occupiedFirstclass - @PassengerCount
                            END
                        FROM Flight f
                        INNER JOIN Tiene t
                            ON t.flightNumber = f.numberFlight
                        WHERE t.transactionId = @TransactionId;
                    ";

                    await _connection.ExecuteAsync(
                        releaseFirstClassSeats,
                        new
                        {
                            TransactionId = transactionId,
                            PassengerCount = passengerCount
                        },
                        dbTransaction
                    );
                }
                else if (seatClass == "Turista")
                {
                    const string releaseTouristSeats = @"
                        UPDATE f
                        SET occupiedTurist =
                            CASE
                                WHEN occupiedTurist - @PassengerCount < 0 THEN 0
                                ELSE occupiedTurist - @PassengerCount
                            END
                        FROM Flight f
                        INNER JOIN Tiene t
                            ON t.flightNumber = f.numberFlight
                        WHERE t.transactionId = @TransactionId;
                    ";

                    await _connection.ExecuteAsync(
                        releaseTouristSeats,
                        new
                        {
                            TransactionId = transactionId,
                            PassengerCount = passengerCount
                        },
                        dbTransaction
                    );
                }

                const string getLuggageTotals = @"
                    SELECT
                        ISNULL(SUM(CASE WHEN l.type = 'checked' THEN l.quantity ELSE 0 END), 0) AS CheckedQuantity,
                        ISNULL(SUM(CASE WHEN l.type = 'carryOn' THEN l.quantity ELSE 0 END), 0) AS CarryOnQuantity
                    FROM Registra r
                    INNER JOIN Luggage l
                        ON l.luggageNumber = r.luggageNumber
                    WHERE r.transactionIdItinerary = @TransactionId;
                ";

                var luggageTotals = await _connection.QuerySingleAsync<dynamic>(
                    getLuggageTotals,
                    new { TransactionId = transactionId },
                    dbTransaction
                );

                decimal checkedQuantity = luggageTotals.CheckedQuantity;
                decimal carryOnQuantity = luggageTotals.CarryOnQuantity;

                const string releaseLuggage = @"
                    UPDATE f
                    SET
                        occupiedLuggage =
                            CASE
                                WHEN occupiedLuggage - @CheckedQuantity < 0 THEN 0
                                ELSE occupiedLuggage - @CheckedQuantity
                            END,
                        occupiedCarryOn =
                            CASE
                                WHEN occupiedCarryOn - @CarryOnQuantity < 0 THEN 0
                                ELSE occupiedCarryOn - @CarryOnQuantity
                            END
                    FROM Flight f
                    INNER JOIN Tiene t
                        ON t.flightNumber = f.numberFlight
                    WHERE t.transactionId = @TransactionId;
                ";

                await _connection.ExecuteAsync(
                    releaseLuggage,
                    new
                    {
                        TransactionId = transactionId,
                        CheckedQuantity = checkedQuantity,
                        CarryOnQuantity = carryOnQuantity
                    },
                    dbTransaction
                );

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
        public async Task<string?> GetBuyerEmailByTransactionIdAsync(string transactionId)
        {
            const string sql = @"
                SELECT TOP 1 p.emailPassenger
                FROM Passenger p
                INNER JOIN PassengerItinerary pi
                    ON pi.idPassenger = p.idPassenger
                WHERE pi.transactionId = @TransactionId
                AND p.emailPassenger IS NOT NULL;
            ";

            return await _connection.QueryFirstOrDefaultAsync<string?>(
                sql,
                new { TransactionId = transactionId }
            );
        }

        public async Task SaveCancellationTokenAsync(
            string transactionId,
            string token,
            DateTime expirationDate)
        {
            const string sql = @"
                INSERT INTO ReservationCancellationToken (
                    transactionId,
                    token,
                    expirationDate
                )
                VALUES (
                    @TransactionId,
                    @Token,
                    @ExpirationDate
                );
            ";

            await _connection.ExecuteAsync(
                sql,
                new
                {
                    TransactionId = transactionId,
                    Token = token,
                    ExpirationDate = expirationDate
                }
            );
        }

        public async Task<(string transactionId, bool isUsed, DateTime expirationDate)?>
            GetCancellationTokenAsync(string token)
        {
            const string sql = @"
                SELECT
                    transactionId,
                    isUsed,
                    expirationDate
                FROM ReservationCancellationToken
                WHERE token = @Token;
            ";

            return await _connection.QueryFirstOrDefaultAsync
                <(string transactionId, bool isUsed, DateTime expirationDate)?>(
                    sql,
                    new { Token = token }
                );
        }

        public async Task MarkCancellationTokenAsUsedAsync(string token)
        {
            const string sql = @"
                UPDATE ReservationCancellationToken
                SET isUsed = 1
                WHERE token = @Token;
            ";

            await _connection.ExecuteAsync(
                sql,
                new { Token = token }
            );
        }
    }
}