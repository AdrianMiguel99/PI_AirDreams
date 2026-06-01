using System.Data;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class LuggageCalculationRepository : ILuggageCalculationRepository
    {
        private readonly IDbConnection _connection;

        public LuggageCalculationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<decimal> CalculateAsync(decimal basePrice, decimal multiplier, int quantity)
        {
            if (quantity <= 0 || basePrice <= 0) return 0m;

            var parameters = new DynamicParameters();
            parameters.Add("base", basePrice);
            parameters.Add("multiplier", multiplier);
            parameters.Add("quantity", quantity);

            return await _connection.ExecuteScalarAsync<decimal>(
                "SELECT dbo.fn_TotalLuggageCost(@base, @multiplier, @quantity)", parameters);
        }
    }
}