using System.Data;
using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly IDbConnection _connection;

        public FlightRepository(IDbConnection connection)
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

        public async Task<IEnumerable<dynamic>> SearchFlightsAsync(
            string origin,
            string destination,
            TimeSpan earliestTime,
            TimeSpan latestTime,
            int quantityOfPassengers
        )
        {
        }
    }
}