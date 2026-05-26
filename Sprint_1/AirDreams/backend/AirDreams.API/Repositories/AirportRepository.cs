using System.Data;
using AirDreams.API.Models.Entities;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly IDbConnection _connection;

        public AirportRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Airport>> GetAllAsync()
        {
            const string sql = "SELECT codeAirport, adminID, nameAirport, city, country, timeZone FROM Airport";
            var airports = await _connection.QueryAsync<Airport>(sql);
            return airports.AsList();
        }

        public async Task<Airport?> GetByCodeAsync(string code)
        {
            const string sql = "SELECT codeAirport, adminID, nameAirport, city, country, timeZone FROM Airport WHERE codeAirport = @Code";
            return await _connection.QueryFirstOrDefaultAsync<Airport>(sql, new { Code = code });
        }

        public async Task<Airport> CreateAsync(Airport airport)
        {
            const string sql = @"INSERT INTO Airport (codeAirport, adminID, nameAirport, city, country, timeZone)
                                VALUES (@CodeAirport, @AdminID, @NameAirport, @City, @Country, @TimeZone)";
            await _connection.ExecuteAsync(sql, airport);
            return airport; 
        }

        public async Task<bool> ExistsAsync(string code)
        {
            const string sql = "SELECT COUNT(1) FROM Airport WHERE codeAirport = @Code";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Code = code });
            return count > 0;
        }

        public async Task<bool> UpdateAsync(string code, string newName)
        {
            var sql = "UPDATE Airport SET nameAirport = @Name WHERE codeAirport = @Code";
            var affected = await _connection.ExecuteAsync(sql, new { Code = code, Name = newName });
            return affected > 0;
        }
    }
}