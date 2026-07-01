using AirDreams.API.DTOs;
using Dapper;
using System.Data;

namespace AirDreams.API.Repositories
{
    public class LuggageRepository : ILuggageRepository
    {
        private readonly IDbConnection _connection;

        public LuggageRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<string?> CreateLuggageAsync(string type, int quantity)
        {
            try
            {
                var luggageNumber = "LUG-" + Guid.NewGuid().ToString("N").Substring(0, 8);

                string query = @"
            INSERT INTO Luggage (luggageNumber, type, quantity)
            VALUES (@luggageNumber, @type, @quantity)";

                var affectedRows = await _connection.ExecuteAsync(query, new
                {
                    luggageNumber,
                    type,
                    quantity
                });

                return affectedRows > 0 ? luggageNumber : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> RegisterLuggageAsync(int idPassenger, string transactionIdItinerary, string luggageNumber)
        {
            try
            {
                string query = @"
                    INSERT INTO Registra (idPassenger, transactionIdItinerary, luggageNumber)
                    VALUES (@idPassenger, @transactionIdItinerary, @luggageNumber)";

                var affectedRows = await _connection.ExecuteAsync(query, new 
                { 
                    idPassenger, 
                    transactionIdItinerary, 
                    luggageNumber 
                });

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckLuggageWeightAsync(string flightId, int routeId, decimal luggageWeight)
        {
            try
            {
                string query = "SELECT dbo.CheckLuggageWeight(@flightId, @routeId, @luggageWeight)";

                var result = await _connection.ExecuteScalarAsync<int>(query, new 
                { 
                    flightId, 
                    routeId, 
                    luggageWeight 
                });

                return result == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckCarryOnWeightAsync(string flightId, int routeId, decimal carryOnWeight)
        {
            try
            {
                string query = "SELECT dbo.CheckCarryOnWeight(@flightId, @routeId, @carryOnWeight)";

                var result = await _connection.ExecuteScalarAsync<int>(query, new 
                { 
                    flightId, 
                    routeId,
                    carryOnWeight 
                });

                return result == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateFlightWeightAsync(string transactionId, decimal luggageWeight, decimal carryOnWeight)
        {
            try
            {
                var result = await _connection.ExecuteScalarAsync<int>(
                    "UpdateItineraryFlightWeight",
                    new { transactionId, luggageWeight, carryOnWeight },
                    commandType: CommandType.StoredProcedure
                );

                return result == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ReservationLuggageResponseDto> GetReservationLuggageAsync(string transactionId)
        {
            string luggageQuery = @"
                SELECT
                    r.idPassenger AS IdPassenger,
                    r.transactionIdItinerary AS TransactionIdItinerary,
                    l.luggageNumber AS LuggageNumber,
                    l.type AS Type,
                    l.quantity AS Quantity
                FROM Registra r
                INNER JOIN Luggage l
                    ON l.luggageNumber = r.luggageNumber
                WHERE r.transactionIdItinerary = @transactionId";

            string segmentsQuery = @"
                SELECT
                    f.numberFlight AS FlightNumber,
                    f.routeId AS RouteId,
                    rt.luggagePrice AS CheckedPrice,
                    rt.carryOnPrice AS CarryOnPrice,
                    rt.porcentageMultiplier AS Multiplier
                FROM Tiene t
                INNER JOIN Flight f
                    ON f.numberFlight = t.flightNumber
                INNER JOIN Route rt
                    ON rt.idRoute = f.routeId
                WHERE t.transactionId = @transactionId";

            var luggage = await _connection.QueryAsync<ReservationLuggageDto>(
                luggageQuery,
                new { transactionId }
            );

            var segments = await _connection.QueryAsync<ReservationLuggageSegmentDto>(
                segmentsQuery,
                new { transactionId }
            );

            return new ReservationLuggageResponseDto
            {
                Luggage = luggage,
                Segments = segments
            };
        }
    }
}
