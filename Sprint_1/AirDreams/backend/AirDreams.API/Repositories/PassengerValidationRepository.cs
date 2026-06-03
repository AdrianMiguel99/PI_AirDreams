using System.Data;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class PassengerValidationRepository : IPassengerValidationRepository
    {
        private readonly IDbConnection _connection;

        public PassengerValidationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<string>> FindDuplicatePassengersAsync(
            IEnumerable<PassengerCheck> passengers,
            IEnumerable<string> flightNumbers)
        {
            var duplicates = new List<string>();
            foreach (var p in passengers)
            {
                var sql = @"
                    SELECT COUNT(1)
                    FROM Passenger p
                    JOIN PassengerItinerary pi ON p.idPassenger = pi.idPassenger
                    JOIN Itinerary i ON pi.transactionId = i.transactionId
                    JOIN Tiene t ON i.transactionId = t.transactionId
                    JOIN Flight f ON t.flightNumber = f.numberFlight
                    WHERE p.namePassenger = @Name
                      AND p.lastnamesPassenger = @Lastnames
                      AND p.country = @Country
                      AND f.numberFlight IN @FlightNumbers";

                var count = await _connection.ExecuteScalarAsync<int>(sql, new
                {
                    Name = p.NamePassenger,
                    Lastnames = p.LastnamesPassenger,
                    Country = p.Country,
                    FlightNumbers = flightNumbers
                });

                if (count > 0)
                {
                    duplicates.Add($"{p.NamePassenger} {p.LastnamesPassenger}");
                }
            }
            return duplicates;
        }
    }
}