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
            const string sql = "SELECT codeAirport, adminID, nameAirport, city, country, timeZone, isActive FROM Airport";
            var airports = await _connection.QueryAsync<Airport>(sql);
            return airports.AsList();
        }

        public async Task<Airport?> GetByCodeAsync(string code)
        {
            const string sql = "SELECT codeAirport, adminID, nameAirport, city, country, timeZone, isActive FROM Airport WHERE codeAirport = @Code";
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

        public async Task<bool> IsAirportInUseAsync(string code)
        {
            var sql = @"SELECT COUNT(1) FROM Route
                WHERE codeAirportSalida = @Code OR codeAirportLlegada = @Code";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { Code = code });
            return count > 0;
        }

        public async Task<bool> SoftDeleteAsync(string code)
        {
            var sql = "UPDATE Airport SET isActive = 0 WHERE codeAirport = @Code";
            var affected = await _connection.ExecuteAsync(sql, new { Code = code });
            return affected > 0;
        }

        public async Task<bool> HardDeleteAsync(string code)
        {
            var sql = "DELETE FROM Airport WHERE codeAirport = @Code";
            var affected = await _connection.ExecuteAsync(sql, new { Code = code });
            return affected > 0;
        }

        public async Task<(bool success, string message)> DeleteAirportAsync(string code, bool inUse)
        {
            if (!inUse)
            {
                var sql = "DELETE FROM Airport WHERE codeAirport = @Code";
                var affected = await _connection.ExecuteAsync(sql, new { Code = code });
                return affected > 0
                    ? (true, "Aeropuerto eliminado permanentemente.")
                    : (false, "No se pudo eliminar el aeropuerto.");
            }
            else
            {
                _connection.Open();
                using var transaction = _connection.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    bool stillInUse = await _connection.ExecuteScalarAsync<bool>(
                        "SELECT COUNT(1) FROM Route WHERE codeAirportSalida = @Code OR codeAirportLlegada = @Code",
                        new { Code = code }, transaction);

                    if (!stillInUse)
                    {
                        await _connection.ExecuteAsync("DELETE FROM Airport WHERE codeAirport = @Code", new { Code = code }, transaction);
                        transaction.Commit();
                        return (true, "Aeropuerto eliminado permanentemente.");
                    }
                    else
                    {
                        await _connection.ExecuteAsync("UPDATE Airport SET isActive = 0 WHERE codeAirport = @Code", new { Code = code }, transaction);
                        transaction.Commit();
                        return (true, "Aeropuerto marcado como inactivo porque está siendo usado por una o más rutas.");
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}