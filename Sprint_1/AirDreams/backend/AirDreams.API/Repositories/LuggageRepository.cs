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
                string query = @"
                    INSERT INTO Luggage (type, quantity)
                    OUTPUT INSERTED.luggageNumber
                    VALUES (@type, @quantity)";

                var luggageNumber = await _connection.ExecuteScalarAsync<string>(query, new 
                { 
                    type, 
                    quantity 
                });

                return luggageNumber;
            }
            catch (Exception ex)
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

        public async Task<bool> CheckLuggageWeightAsync(string flightId, decimal luggageWeight)
        {
            try
            {
                string query = "SELECT dbo.CheckLuggageWeight(@flightId, @luggageWeight)";

                var result = await _connection.ExecuteScalarAsync<int>(query, new 
                { 
                    flightId, 
                    luggageWeight 
                });

                return result == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckCarryOnWeightAsync(string flightId, decimal carryOnWeight)
        {
            try
            {
                string query = "SELECT dbo.CheckCarryOnWeight(@flightId, @carryOnWeight)";

                var result = await _connection.ExecuteScalarAsync<int>(query, new 
                { 
                    flightId, 
                    carryOnWeight 
                });

                return result == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
