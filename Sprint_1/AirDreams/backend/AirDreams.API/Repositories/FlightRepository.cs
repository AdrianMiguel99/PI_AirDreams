using System.Data;
using Dapper;
using AirDreams.API.Repositories;

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
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        )
        {
            const string sql = @"
                SELECT 
                    f.numberFlight AS FlightNumber,
                    r.idRoute AS RouteId,
                    ff.dayOfWeek AS DayOfWeek,
                    CAST(ff.startingDate AS DATE) AS OperatingDate,
                    CONVERT(VARCHAR(8), ff.departureTime, 108) AS DepartureTime,
                    CONVERT(VARCHAR(8), ff.estimatedArrivalTime, 108) AS ArrivalTime,
                    r.stimatedTime AS Duration,
                    r.turistClassPrice AS TouristPrice,
                    r.firstClassPrice AS FirstClassPrice,
                    f.priceLuggage AS CarryOnPrice,
                    f.priceLuggage AS CheckedPrice,
                    a1.codeAirport AS DepartureAirportCode,
                    a1.nameAirport AS DepartureAirportName,
                    a1.city AS DepartureCity,
                    a2.codeAirport AS ArrivalAirportCode,
                    a2.nameAirport AS ArrivalAirportName,
                    a2.city AS ArrivalCity

                FROM Flight f
                INNER JOIN Route r ON f.routeId = r.idRoute
                INNER JOIN FlightFrequency ff ON r.idRoute = ff.idRoute
                INNER JOIN Airport a1 ON r.codeAirportSalida = a1.codeAirport
                INNER JOIN Airport a2 ON r.codeAirportLlegada = a2.codeAirport
                INNER JOIN Aircraft ac ON r.plateNumber = ac.plateNumber

                WHERE r.codeAirportSalida = @origin
                  AND r.codeAirportLlegada = @destination
                  AND ac.cantPasajeros >= @quantityOfPassengers
                  AND ff.active = 1
                  AND ff.startingDate <= CAST(@searchEndDate AS DATE)
                  AND ff.endingDate >= CAST(@searchStartDate AS DATE)
                  AND ff.departureTime >= CAST(@searchStartTime AS TIME)
                  AND ff.departureTime <= CAST(@searchEndTime AS TIME)
            ";

            var searchStartDate = earliestDeparture.Date;
            var searchEndDate = latestDeparture.Date;
            var searchStartTime = earliestDeparture.TimeOfDay;
            var searchEndTime = latestDeparture.TimeOfDay;

            return await _connection.QueryAsync<dynamic>(sql, new
            {
                origin,
                destination,
                quantityOfPassengers,
                searchStartDate,
                searchEndDate,
                searchStartTime,
                searchEndTime
            });
        }
    }
}