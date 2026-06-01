using System.Data;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly IDbConnection _connection;

        public SecurityRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<string?> ValidateApiKeyAsync(string apiKey)
        {
            const string sql = @"
                SELECT eu.airlineName
                FROM ExternalUser eu
                WHERE eu.secretKey = @apiKey
            ";

            return await _connection.QueryFirstOrDefaultAsync<string>(sql, new { apiKey });
        }
    }
}
