using System.Data;
using Dapper;
using AirDreams.ExternalAPI.DTOs;
using AirDreams.API.Repositories.Interfaces;

namespace AirDreams.API.Repositories
{
    public class ExternalFlightRepository : IExternalFlightRepository
    {
        private readonly IDbConnection _connection;

        private static readonly Dictionary<string, string> PartnerNames = new()
        {
            { "AD", "AirDreams" },
            { "MU", "Mushu Airlines" },
            { "SN", "Snoopy Airlines" },
            { "ZU", "Zuli Airlines" }
        };

        public ExternalFlightRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> ExistsAsync(string flightNumber)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM ExternalFlight
                WHERE flightNumber = @flightNumber";

            return await _connection.ExecuteScalarAsync<int>(sql, new { flightNumber }) > 0;
        }

        public async Task RegisterExternalFlightAsync(
            string transactionId,
            ExternalResponseFlightDTO flight,
            string partnerName)
        {
            var departure = DateTime.Parse(flight.departureTime);
            var arrival = DateTime.Parse(flight.arrivalTime);

            if (!await ExistsAsync(flight.flightGUID))
            {
                const string insertFlight = @"
                INSERT INTO ExternalFlight
                (
                    flightNumber,
                    partnerName,
                    departureAirportCode,
                    arrivalAirportCode,
                    departureDateTime,
                    arrivalDateTime,
                    duration,
                    touristPrice,
                    firstClassPrice,
                    carryOnPrice,
                    checkedPrice
                )
                VALUES
                (
                    @flightNumber,
                    @partnerName,
                    @departureAirportCode,
                    @arrivalAirportCode,
                    @departureDateTime,
                    @arrivalDateTime,
                    @duration,
                    @touristPrice,
                    @firstClassPrice,
                    @carryOnPrice,
                    @checkedPrice
                )";

                await _connection.ExecuteAsync(insertFlight, new
                {
                    flightNumber = flight.flightGUID,
                    partnerName,
                    departureAirportCode = flight.departureAirport.code,
                    arrivalAirportCode = flight.arrivalAirport.code,
                    departureDateTime = departure,
                    arrivalDateTime = arrival,
                    duration = TimeSpan.Parse(flight.duration),
                    touristPrice = flight.touristPrice,
                    firstClassPrice = flight.firstClassPrice,
                    carryOnPrice = flight.carryOnPrice,
                    checkedPrice = flight.checkedPrice
                });
            }

            await _connection.ExecuteAsync(@"
                INSERT INTO TieneExternal
                (
                    transactionId,
                    externalFlightNumber
                )
                VALUES
                (
                    @transactionId,
                    @flightNumber
                )",
                new
                {
                    transactionId,
                    flightNumber = flight.flightGUID
                });

            await _connection.ExecuteAsync(@"
                UPDATE Itinerary
                SET hasExternalFlight = 1
                WHERE transactionId = @transactionId",
                new
                {
                    transactionId
                }
            );
        }
    }
}