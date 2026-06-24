using AirDreams.API.DTOs;
using AirDreams.API.Models.Dtos;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace AirDreams.API.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly IDbConnection _connection;

        public PurchaseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> CheckFlightAvailabilityAsync(string numberFlight, string seatClass, int requestedSeats)
        {
            var result = await _connection.ExecuteScalarAsync<int>(
                "CheckFlightAvailability",
                new { NumberFlight = numberFlight, SeatClass = seatClass, RequestedSeats = requestedSeats },
                commandType: CommandType.StoredProcedure
            );
            return result == 1;
        }

        public async Task ConfirmPurchaseAsync(ConfirmPurchaseDto dto, string? cardLastFour)
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                var luggageCostParams = new DynamicParameters();
                luggageCostParams.Add("Segments", BuildSegmentTable(dto).AsTableValuedParameter("dbo.FlightSegmentType"));
                luggageCostParams.Add("Luggage", BuildLuggageTable(dto).AsTableValuedParameter("dbo.LuggagePurchaseType"));
                luggageCostParams.Add("TotalCost", dbType: DbType.Decimal, direction: ParameterDirection.Output);

                await _connection.ExecuteAsync("dbo.sp_CalculateLuggageCost", luggageCostParams, transaction, commandType: CommandType.StoredProcedure);
                decimal luggageCost = luggageCostParams.Get<decimal>("TotalCost");

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

                decimal flightCost = dto.PassengerCount * dto.PricePerPassenger;
                decimal amount = flightCost + luggageCost;
                var firstPassengerId = passengerMappings.First().IdPassenger;

                var sqlItinerary = @"
                    INSERT INTO Itinerary (transactionId, idPassenger, purchaseDate, amount, seatClass)
                    VALUES (@TransactionId, @IdPassenger, GETDATE(), @Amount, @SeatClass)";

                await _connection.ExecuteAsync(sqlItinerary, new
                {
                    dto.TransactionId,
                    IdPassenger = firstPassengerId,
                    Amount = amount,
                    SeatClass = dto.SeatClass
                }, transaction);

                var passengerItineraryValues = passengerMappings
                    .Select(pm => $"({pm.IdPassenger}, '{dto.TransactionId}')");
                var sqlPassengerItinerary = $@"
                    INSERT INTO PassengerItinerary (idPassenger, transactionId)
                    VALUES {string.Join(", ", passengerItineraryValues)}";

                await _connection.ExecuteAsync(sqlPassengerItinerary, transaction: transaction);

                if (luggageMappings.Any())
                {
                    var registraValues = luggageMappings.Select(lm =>
                    {
                        var passengerId = passengerMappings
                            .First(pm => pm.PassengerIndex == lm.PassengerIndex).IdPassenger;
                        return $"({passengerId}, '{dto.TransactionId}', '{lm.LuggageNumber}')";
                    });

                    var sqlRegistra = $@"
                        INSERT INTO Registra (idPassenger, transactionIdItinerary, luggageNumber)
                        VALUES {string.Join(", ", registraValues)}";

                    await _connection.ExecuteAsync(sqlRegistra, transaction: transaction);
                }

                var segmentTable = BuildSegmentTable(dto);
                await _connection.ExecuteAsync(
                    "dbo.sp_InsertFlightSegments",
                    new
                    {
                        TransactionId = dto.TransactionId,
                        Segments = segmentTable.AsTableValuedParameter("dbo.FlightSegmentType"),
                        SeatClass = dto.SeatClass,
                        PassengerCount = dto.PassengerCount
                    },
                    transaction,
                    commandType: CommandType.StoredProcedure
                );

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

        private static DataTable BuildPassengerTable(ConfirmPurchaseDto dto)
        {
            var table = new DataTable();
            table.Columns.Add("PassengerIndex", typeof(int));
            table.Columns.Add("NamePassenger", typeof(string));
            table.Columns.Add("LastnamesPassenger", typeof(string));
            table.Columns.Add("EmailPassenger", typeof(string));
            table.Columns.Add("Telephone", typeof(string));
            table.Columns.Add("Country", typeof(string));
            table.Columns.Add("BirthDate", typeof(DateTime));

            for (int i = 0; i < dto.Passengers.Count; i++)
            {
                var p = dto.Passengers[i];
                DateTime? birth = null;
                if (!string.IsNullOrWhiteSpace(p.BirthDate))
                {
                    if (DateTime.TryParseExact(p.BirthDate, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                        birth = parsed;
                }
                table.Rows.Add(
                    i + 1,
                    p.NamePassenger,
                    p.LastnamesPassenger,
                    string.IsNullOrWhiteSpace(p.EmailPassenger) ? DBNull.Value : p.EmailPassenger.Trim().ToLower(),
                    p.Telephone ?? string.Empty,
                    p.Country,
                    birth ?? (object)DBNull.Value
                );
            }
            return table;
        }

        private static DataTable BuildLuggageTable(ConfirmPurchaseDto dto)
        {
            var table = new DataTable();
            table.Columns.Add("PassengerIndex", typeof(int));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Quantity", typeof(byte));

            if (dto.Luggage == null) return table;

            foreach (var pl in dto.Luggage)
            {
                foreach (var item in pl.LuggageItems)
                {
                    if (item.Quantity > 0)
                    {
                        table.Rows.Add(pl.PassengerIndex, item.Type, Convert.ToByte(item.Quantity));
                    }
                }
            }
            return table;
        }

        private static DataTable BuildSegmentTable(ConfirmPurchaseDto dto)
        {
            var table = new DataTable();
            table.Columns.Add("FlightNumber", typeof(string));
            table.Columns.Add("RouteId", typeof(int));
            table.Columns.Add("CheckedPrice", typeof(decimal));
            table.Columns.Add("CarryOnPrice", typeof(decimal));
            table.Columns.Add("Multiplier", typeof(decimal));

            foreach (var s in dto.Segments)
            {
                var flight = s.FlightNumber ?? "";
                if (flight.Length > 50) flight = flight.Substring(0, 50);

                table.Rows.Add(flight, s.RouteId ?? 1, s.CheckedPrice, s.CarryOnPrice, s.Multiplier);
            }
            return table;
        }

        public async Task<Dictionary<string, decimal>> GetMultipliersByFlightsAsync(IEnumerable<string> flightNumbers)
        {
            var sql = @"SELECT f.numberFlight, r.porcentageMultiplier
                FROM Flight f
                JOIN Route r ON f.routeId = r.idRoute
                WHERE f.numberFlight IN @FlightNumbers";
            var result = await _connection.QueryAsync(sql, new { FlightNumbers = flightNumbers });
            return result.ToDictionary(
                row => (string)row.numberFlight,
                row => (decimal)row.porcentageMultiplier
            );
        }
    }
}