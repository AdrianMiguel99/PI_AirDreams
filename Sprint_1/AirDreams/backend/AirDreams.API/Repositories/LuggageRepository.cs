using System.Data;
using Dapper;

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
                string query = @"
                    UPDATE f
                    SET
                        occupiedLuggage = occupiedLuggage + @luggageWeight,
                        occupiedCarryOn = occupiedCarryOn + @carryOnWeight
                    FROM Flight f
                    INNER JOIN Tiene t
                        ON t.flightNumber = f.numberFlight
                    WHERE t.transactionId = @transactionId";

                var rows = await _connection.ExecuteAsync(query, new {
                        transactionId,
                        luggageWeight,
                        carryOnWeight
                    });

                return rows > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
