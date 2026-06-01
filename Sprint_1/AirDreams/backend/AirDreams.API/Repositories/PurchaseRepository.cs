using System.Data;
using Dapper;
using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly IDbConnection _connection;

        public PurchaseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task ConfirmPurchaseAsync(ConfirmPurchaseDto dto, decimal amount, string? cardLastFour)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();

            try
            {
                var passengerIds = new List<int>();
                foreach (var p in dto.Passengers)
                {
                    int newId = await _connection.ExecuteScalarAsync<int>(
                        "SELECT ISNULL(MAX(idPassenger), 0) + 1 FROM Passenger",
                        transaction: transaction);

                    var sqlPassenger = @"
                        INSERT INTO Passenger (idPassenger, namePassenger, lastnamesPassenger, emailPassenger, telephone, country)
                        VALUES (@IdPassenger, @NamePassenger, @LastnamesPassenger, @EmailPassenger, @Telephone, @Country)";

                    await _connection.ExecuteAsync(sqlPassenger, new
                    {
                        IdPassenger = newId,
                        p.NamePassenger,
                        p.LastnamesPassenger,
                        p.EmailPassenger,
                        Telephone = p.Telephone ?? (object)DBNull.Value,
                        Country = p.Country
                    }, transaction);

                    passengerIds.Add(newId);
                }

                var sqlItinerary = @"
                    INSERT INTO Itinerary (transactionId, idPassenger, purchaseDate, amount)
                    VALUES (@TransactionId, @IdPassenger, GETDATE(), @Amount)";

                await _connection.ExecuteAsync(sqlItinerary, new
                {
                    dto.TransactionId,
                    IdPassenger = passengerIds.First(),
                    Amount = amount
                }, transaction);

                foreach (var pid in passengerIds)
                {
                    var sqlPI = @"
                        INSERT INTO PassengerItinerary (idPassenger, transactionId)
                        VALUES (@IdPassenger, @TransactionId)";
                    await _connection.ExecuteAsync(sqlPI, new
                    {
                        IdPassenger = pid,
                        dto.TransactionId
                    }, transaction);
                }

                foreach (var segment in dto.Segments)
                {
                    await EnsureFlightExistsAsync(transaction, segment.FlightNumber, segment.RouteId);

                    var sqlTiene = @"
                        INSERT INTO Tiene (transactionId, flightNumber)
                        VALUES (@TransactionId, @FlightNumber)";
                    await _connection.ExecuteAsync(sqlTiene, new
                    {
                        dto.TransactionId,
                        segment.FlightNumber
                    }, transaction);
                }

                if (dto.Luggage != null)
                {
                    for (int i = 0; i < dto.Luggage.Count; i++)
                    {
                        var lp = dto.Luggage[i];
                        int passengerId = passengerIds[lp.PassengerIndex - 1];

                        foreach (var item in lp.LuggageItems)
                        {
                            var luggageNumber = $"LUG-{dto.TransactionId}-{lp.PassengerIndex}-{item.Type}";
                            var sqlLuggage = @"
                                INSERT INTO Luggage (luggageNumber, type, quantity)
                                VALUES (@LuggageNumber, @Type, @Quantity)";
                            await _connection.ExecuteAsync(sqlLuggage, new
                            {
                                LuggageNumber = luggageNumber,
                                Type = item.Type,
                                Quantity = item.Quantity
                            }, transaction);

                            var sqlRegistra = @"
                                INSERT INTO Registra (idPassenger, transactionIdItinerary, luggageNumber)
                                VALUES (@IdPassenger, @TransactionId, @LuggageNumber)";
                            await _connection.ExecuteAsync(sqlRegistra, new
                            {
                                IdPassenger = passengerId,
                                TransactionId = dto.TransactionId,
                                LuggageNumber = luggageNumber
                            }, transaction);
                        }
                    }
                }

                var sqlPayment = @"
                    UPDATE Itinerary
                    SET paymentMethod = @PaymentMethod,
                        cardLastFour = @CardLastFour,
                        buyerName = @BuyerName,
                        paymentDate = GETDATE()
                    WHERE transactionId = @TransactionId";
                await _connection.ExecuteAsync(sqlPayment, new
                {
                    dto.TransactionId,
                    dto.PaymentMethod,
                    CardLastFour = cardLastFour,
                    dto.BuyerName
                }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task EnsureFlightExistsAsync(IDbTransaction transaction,
            string flightNumber, int? routeId)
        {
            var exists = await _connection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) FROM Flight WHERE numberFlight = @FlightNumber",
                new { FlightNumber = flightNumber }, transaction);

            if (!exists)
            {
                int resolvedRouteId = routeId ?? 1;
                var sqlInsertFlight = @"
                    INSERT INTO Flight (numberFlight, routeId, boardingGate, departureDate, flightState)
                    VALUES (@FlightNumber, @RouteId, 1, CAST(GETDATE() AS DATE), 'On-time')";
                await _connection.ExecuteAsync(sqlInsertFlight, new
                {
                    FlightNumber = flightNumber,
                    RouteId = resolvedRouteId
                }, transaction);
            }
        }
    }
}