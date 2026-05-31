using System.Data;
using Dapper;
using AirDreams.API.Models.Entities;

namespace AirDreams.API.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly IDbConnection _connection;

        public PassengerRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> ExistsAsync(int idPassenger)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM Passenger
                WHERE idPassenger = @IdPassenger;
            ";

            var count = await _connection.ExecuteScalarAsync<int>(sql, new
            {
                IdPassenger = idPassenger
            });

            return count > 0;
        }

        public async Task<Passenger?> GetByIdAsync(int idPassenger)
        {
            const string sql = @"
                SELECT
                    idPassenger AS IdPassenger,
                    namePassenger AS NamePassenger,
                    lastnamesPassenger AS LastnamesPassenger,
                    emailPassenger AS EmailPassenger,
                    telephone AS Telephone,
                    countryCode AS CountryCode
                FROM Passenger
                WHERE idPassenger = @IdPassenger;
            ";

            return await _connection.QueryFirstOrDefaultAsync<Passenger>(sql, new
            {
                IdPassenger = idPassenger
            });
        }

        public async Task<Passenger> CreateAsync(Passenger passenger)
        {
            const string sql = @"
                INSERT INTO Passenger (
                    idPassenger,
                    namePassenger,
                    lastnamesPassenger,
                    emailPassenger,
                    telephone,
                    countryCode
                )
                VALUES (
                    @IdPassenger,
                    @NamePassenger,
                    @LastnamesPassenger,
                    @EmailPassenger,
                    @Telephone,
                    @CountryCode
                );
            ";

            await _connection.ExecuteAsync(sql, passenger);
            return passenger;
        }

        public async Task<Passenger> GetPassengerByNameAsync(string name)
        {
            const string sql = @"
                SELECT
                    idPassenger AS IdPassenger,
                    namePassenger AS NamePassenger,
                    lastnamesPassenger AS LastnamesPassenger,
                    emailPassenger AS EmailPassenger,
                    telephone AS Telephone,
                    countryCode AS CountryCode
                FROM Passenger
                WHERE namePassenger = @Name;
            ";

            return await _connection.QueryFirstOrDefaultAsync<Passenger>(sql, new
            {
                Name = name
            });
        }
    }
}
