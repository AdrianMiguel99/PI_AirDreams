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
            WITH SearchDates AS (
                SELECT CAST(@searchStartDate AS DATE) AS SearchDate
                UNION ALL
                SELECT DATEADD(DAY, 1, SearchDate)
                FROM SearchDates
                WHERE SearchDate < CAST(@searchEndDate AS DATE)
            )
            SELECT
                    CONCAT('R', r.idRoute, 'F', ff.idFrequency, '-', CONVERT(CHAR(8), sd.SearchDate, 112)) AS FlightNumber,
                    r.idRoute AS RouteId,
                    sd.SearchDate AS DepartureDate,
                    CONVERT(VARCHAR(8), ff.departureTime, 108) AS DepartureTime,
                    CONVERT(VARCHAR(8), ff.estimatedArrivalTime, 108) AS ArrivalTime,
                    r.stimatedTime AS Duration,
                    r.turistClassPrice AS TouristPrice,
                    r.firstClassPrice AS FirstClassPrice,
                    CAST(0 AS DECIMAL(10, 2)) AS CarryOnPrice,
                    CAST(0 AS DECIMAL(10, 2)) AS CheckedPrice,
                    a1.codeAirport AS DepartureAirportCode,
                    a1.nameAirport AS DepartureAirportName,
                    a1.city AS DepartureCity,
                    a2.codeAirport AS ArrivalAirportCode,
                    a2.nameAirport AS ArrivalAirportName,
                    a2.city AS ArrivalCity,
                    ac.cantPasajeros AS AvailableSeats

                FROM SearchDates sd
                INNER JOIN Route r ON r.codeAirportSalida = @origin
                INNER JOIN FlightFrequency ff ON r.idRoute = ff.idRoute
                INNER JOIN Airport a1 ON r.codeAirportSalida = a1.codeAirport
                INNER JOIN Airport a2 ON r.codeAirportLlegada = a2.codeAirport
                INNER JOIN Aircraft ac ON r.plateNumber = ac.plateNumber
                CROSS APPLY (
                    SELECT DATEADD(
                        SECOND,
                        DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ff.departureTime),
                        CAST(sd.SearchDate AS DATETIME)
                    ) AS DepartureDateTime
                ) searchedFlight

                WHERE r.codeAirportLlegada = @destination
                AND ac.cantPasajeros >= @quantityOfPassengers
                AND ff.active = 1
                AND sd.SearchDate >= ff.startingDate
                AND sd.SearchDate <= ff.endingDate
                AND ff.dayOfWeek = CASE DATEDIFF(DAY, '19000101', sd.SearchDate) % 7
                    WHEN 0 THEN 'Monday'
                    WHEN 1 THEN 'Tuesday'
                    WHEN 2 THEN 'Wednesday'
                    WHEN 3 THEN 'Thursday'
                    WHEN 4 THEN 'Friday'
                    WHEN 5 THEN 'Saturday'
                    WHEN 6 THEN 'Sunday'
                END
                AND searchedFlight.DepartureDateTime >= @earliestDeparture
                AND searchedFlight.DepartureDateTime <= @latestDeparture
                OPTION (MAXRECURSION 366)
            ";

            var searchStartDate = earliestDeparture.Date;
            var searchEndDate = latestDeparture.Date;

            return await _connection.QueryAsync<dynamic>(sql, new
            {
                origin,
                destination,
                quantityOfPassengers,
                searchStartDate,
                searchEndDate,
                earliestDeparture,
                latestDeparture
            });
        }
    }
}
