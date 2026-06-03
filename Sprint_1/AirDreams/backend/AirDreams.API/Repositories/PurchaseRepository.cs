using AirDreams.API.DTOs;
using AirDreams.API.Models.Dtos;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AirDreams.API.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly IDbConnection _connection;

        public PurchaseRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<bool> CheckFlightAvailabilityAsync( string numberFlight, string seatClass, int requestedSeats)
        {
            var result = await _connection.ExecuteScalarAsync<int>(
                "CheckFlightAvailability",
                new { NumberFlight = numberFlight, SeatClass = seatClass, RequestedSeats = requestedSeats}, commandType: CommandType.StoredProcedure 
            );

            return result == 1;
        }

        public async Task ReserveFlightSeatsAsync(string numberFlight, string seatClass, int requestedSeats, IDbTransaction transaction)
        {
            await _connection.ExecuteAsync(
                "ReserveFlightSeats",
                new
                {
                    NumberFlight = numberFlight,
                    SeatClass = seatClass,
                    RequestedSeats = requestedSeats
                },
                transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task ConfirmPurchaseAsync(ConfirmPurchaseDto dto, string? cardLastFour)
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                decimal flightCost = dto.PassengerCount * dto.PricePerPassenger;
                decimal luggageCost = await CalculateLuggageCostAsync(dto, transaction);

                var passengerTable = BuildPassengerTable(dto);
                var luggageTable = BuildLuggageTable(dto);

                var spResult = await _connection.QueryMultipleAsync(
                    "dbo.sp_InsertPassengersAndLuggage",
                    new
                    {
                        TransactionId = dto.TransactionId,
                        Passengers = passengerTable.AsTableValuedParameter("dbo.PassengerPurchaseType"),
                        Luggage = luggageTable.AsTableValuedParameter("dbo.LuggagePurchaseType")
                    },
                    transaction,
                    commandType: CommandType.StoredProcedure
                );

                var passengerMappings = (await spResult.ReadAsync<PassengerMappingDto>()).ToList();
                var luggageMappings = (await spResult.ReadAsync<LuggageMappingDto>()).ToList();

                if (!passengerMappings.Any())
                    throw new InvalidOperationException("No se registraron pasajeros para la compra.");

                var passengerIds = passengerMappings
                    .OrderBy(p => p.PassengerIndex)
                    .Select(p => p.IdPassenger)
                    .ToList();

                decimal amount = flightCost + luggageCost;

                var sqlItinerary = @"
                    INSERT INTO Itinerary (transactionId, idPassenger, purchaseDate, amount)
                    VALUES (@TransactionId, @IdPassenger, GETDATE(), @Amount)";

                await _connection.ExecuteAsync(sqlItinerary, new
                {
                    dto.TransactionId,
                    IdPassenger = passengerIds.First(),
                    Amount = amount
                }, transaction);

                foreach (var passenger in passengerMappings)
                {
                    var sqlPI = @"
                        INSERT INTO PassengerItinerary (idPassenger, transactionId)
                        VALUES (@IdPassenger, @TransactionId)";

                    await _connection.ExecuteAsync(sqlPI, new
                    {
                        passenger.IdPassenger,
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

                    await ReserveFlightSeatsAsync(
                        segment.FlightNumber,
                        dto.SeatClass,
                        dto.PassengerCount,
                        transaction);
                }

                foreach (var luggage in luggageMappings)
                {
                    var passengerId = passengerMappings
                        .First(p => p.PassengerIndex == luggage.PassengerIndex)
                        .IdPassenger;

                    var sqlRegistra = @"
                        INSERT INTO Registra (idPassenger, transactionIdItinerary, luggageNumber)
                        VALUES (@IdPassenger, @TransactionId, @LuggageNumber)";

                    await _connection.ExecuteAsync(sqlRegistra, new
                    {
                        IdPassenger = passengerId,
                        TransactionId = dto.TransactionId,
                        luggage.LuggageNumber
                    }, transaction);
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

        private async Task<decimal> CalculateLuggageCostAsync(ConfirmPurchaseDto dto, IDbTransaction transaction)
        {
            decimal luggageCost = 0;

            if (dto.Luggage == null)
                return luggageCost;

            foreach (var passengerLuggage in dto.Luggage)
            {
                foreach (var item in passengerLuggage.LuggageItems)
                {
                    if (item.Quantity <= 0)
                        continue;

                    foreach (var segment in dto.Segments)
                    {
                        decimal basePrice = item.Type == "checked"
                            ? segment.CheckedPrice
                            : segment.CarryOnPrice;

                        var cost = await _connection.ExecuteScalarAsync<decimal>(
                            "SELECT dbo.fn_TotalLuggageCost(@basePrice, @multiplier, @quantity)",
                            new
                            {
                                basePrice,
                                multiplier = segment.Multiplier,
                                quantity = item.Quantity
                            },
                            transaction);

                        luggageCost += cost;
                    }
                }
            }

            return luggageCost;
        }

        private static DataTable BuildPassengerTable(ConfirmPurchaseDto dto)
        {
            var passengerTable = new DataTable();
            passengerTable.Columns.Add("PassengerIndex", typeof(int));
            passengerTable.Columns.Add("NamePassenger", typeof(string));
            passengerTable.Columns.Add("LastnamesPassenger", typeof(string));
            passengerTable.Columns.Add("EmailPassenger", typeof(string));
            passengerTable.Columns.Add("Telephone", typeof(string));
            passengerTable.Columns.Add("Country", typeof(string));

            for (int i = 0; i < dto.Passengers.Count; i++)
            {
                var passenger = dto.Passengers[i];

                passengerTable.Rows.Add(
                    i + 1,
                    passenger.NamePassenger,
                    passenger.LastnamesPassenger,
                    string.IsNullOrWhiteSpace(passenger.EmailPassenger)
                        ? DBNull.Value
                        : passenger.EmailPassenger.Trim().ToLower(),
                    passenger.Telephone ?? string.Empty,
                    passenger.Country
                );
            }

            return passengerTable;
        }

        private static DataTable BuildLuggageTable(ConfirmPurchaseDto dto)
        {
            var luggageTable = new DataTable();
            luggageTable.Columns.Add("PassengerIndex", typeof(int));
            luggageTable.Columns.Add("Type", typeof(string));
            luggageTable.Columns.Add("Quantity", typeof(byte));

            if (dto.Luggage == null)
                return luggageTable;

            foreach (var passengerLuggage in dto.Luggage)
            {
                foreach (var item in passengerLuggage.LuggageItems)
                {
                    if (item.Quantity <= 0)
                        continue;

                    luggageTable.Rows.Add(
                        passengerLuggage.PassengerIndex,
                        item.Type,
                        Convert.ToByte(item.Quantity)
                    );
                }
            }

            return luggageTable;
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
