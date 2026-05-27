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
                    CAST(searchedFlight.ArrivalDateTime AS DATE) AS ArrivalDate,
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
                    ) AS DepartureDateTime,
                    DATEADD(
                        SECOND,
                        DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ff.departureTime) + 
                        DATEDIFF(SECOND, CAST('00:00:00' AS TIME), CAST(r.stimatedTime AS TIME)),
                        CAST(sd.SearchDate AS DATETIME)
                    ) AS ArrivalDateTime
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
        

        public async Task<IEnumerable<dynamic>> SearchOneStopFlightsAsync(
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
                CONCAT(
                'R', r1.idRoute,
                'F', ff1.idFrequency,
                '-R', r2.idRoute,
                'F', ff2.idFrequency,
                '-',
                CONVERT(CHAR(8),
                sd.SearchDate, 112)
                ) AS FlightNumber,

                r1.idRoute AS FirstRouteID,
                r2.idRoute AS SecondRouteID,

                a1.codeAirport AS FirstDepartureAirportCode,
                a1.nameAirport AS FirstDepartureAirportName,
                a1.city AS FirstDepartureCity,

                a2.codeAirport AS FirstArrivalAirportCode,
                a2.nameAirport AS FirstArrivalAirportName,
                a2.city AS FirstArrivalCity,

                a3.codeAirport AS FinalArrivalAirportCode,
                a3.nameAirport AS FinalArrivalAirportName,
                a3.city AS FinalArrivalCity,


                sd.SearchDate AS FirstDepartureDate,
                CAST(firstFlight.FirstArrivalDateTime AS DATE) AS FirstArrivalDate,
                CONVERT(VARCHAR(8), ff1.departureTime, 108) AS FirstDepartureTime,
                CONVERT(VARCHAR(8), ff1.estimatedArrivalTime, 108) AS FirstArrivalTime,
                r1.stimatedTime AS FirstDuration,


                CAST(secondFlight.SecondDepartureDateTime AS DATE) AS SecondDepartureDate,
                CAST(secondFlight.SecondArrivalDateTime AS DATE) AS SecondArrivalDate,
                CONVERT(VARCHAR(8), ff2.departureTime, 108) AS SecondDepartureTime,
                CONVERT(VARCHAR(8), ff2.estimatedArrivalTime, 108) AS SecondArrivalTime,
                r2.stimatedTime AS SecondDuration,

                r1.turistClassPrice + r2.turistClassPrice AS TouristPrice,
                r1.firstClassPrice + r2.firstClassPrice AS FirstClassPrice,

                CAST(0 AS DECIMAL(10, 2)) AS CarryOnPrice,
                CAST(0 AS DECIMAL(10, 2)) AS CheckedPrice,

                CASE
                    WHEN ac1.cantPasajeros < ac2.cantPasajeros THEN ac1.cantPasajeros
                    ELSE ac2.cantPasajeros
                END AS AvailableSeats
                

                FROM SearchDates sd
                    INNER JOIN Route r1 ON r1.codeAirportSalida = @origin

                    INNER JOIN Route r2 ON r1.codeAirportLlegada = r2.codeAirportSalida
                    AND r2.codeAirportLlegada = @destination

                    INNER JOIN FlightFrequency ff1 ON r1.idRoute = ff1.idRoute
                    INNER JOIN FlightFrequency ff2 ON r2.idRoute = ff2.idRoute

                    INNER JOIN Airport a1 ON r1.codeAirportSalida = a1.codeAirport
                    INNER JOIN Airport a2 ON r1.codeAirportLlegada = a2.codeAirport
                    INNER JOIN Airport a3 ON r2.codeAirportLlegada = a3.codeAirport

                    INNER JOIN Aircraft ac1 ON r1.plateNumber = ac1.plateNumber
                    INNER JOIN Aircraft ac2 ON r2.plateNumber = ac2.plateNumber


                    CROSS APPLY (
                        SELECT
                            DATEADD(
                                SECOND,
                                DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ff1.departureTime),
                                CAST(sd.SearchDate AS DATETIME)
                            ) AS FirstDepartureDateTime,
                            DATEADD(
                                SECOND,
                                DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ff1.estimatedArrivalTime),
                                CAST(sd.SearchDate AS DATETIME)
                            ) AS FirstArrivalDateTime
                    ) firstFlight

                    CROSS APPLY (
                        SELECT
                            CASE
                                WHEN ff2.departureTime >= ff1.estimatedArrivalTime
                                    THEN DATEADD(
                                            SECOND,
                                            DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ff2.departureTime),
                                            CAST(sd.SearchDate AS DATETIME)
                                        )
                                ELSE DATEADD(
                                            SECOND,
                                            DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ff2.departureTime),
                                            DATEADD(DAY, 1, CAST(sd.SearchDate AS DATETIME))
                                        )
                            END AS SecondDepartureDateTime,
                            DATEADD(
                                SECOND,
                                DATEDIFF(SECOND, CAST('00:00:00' AS TIME), ff2.departureTime) + 
                                DATEDIFF(SECOND, CAST('00:00:00' AS TIME), CAST(r2.stimatedTime AS TIME)),
                                CASE
                                    WHEN ff2.departureTime >= ff1.estimatedArrivalTime
                                    THEN CAST(sd.SearchDate AS DATETIME)
                                    ELSE DATEADD(DAY, 1, CAST(sd.SearchDate AS DATETIME))
                                END
                            ) AS SecondArrivalDateTime
                    ) secondFlight

                WHERE
                    ac1.cantPasajeros >= @quantityOfPassengers
                    AND ac2.cantPasajeros >= @quantityOfPassengers

                    AND ff1.active = 1
                    AND ff2.active = 1


                    AND sd.SearchDate >= ff1.startingDate
                    AND sd.SearchDate <= ff1.endingDate

                    AND CAST(secondFlight.SecondDepartureDateTime AS DATE) >= ff2.startingDate
                    AND CAST(secondFlight.SecondDepartureDateTime AS DATE) <= ff2.endingDate

                AND ff1.dayOfWeek = CASE DATEDIFF(DAY, '19000101', sd.SearchDate) % 7
                    WHEN 0 THEN 'Monday'
                    WHEN 1 THEN 'Tuesday'
                    WHEN 2 THEN 'Wednesday'
                    WHEN 3 THEN 'Thursday'
                    WHEN 4 THEN 'Friday'
                    WHEN 5 THEN 'Saturday'
                    WHEN 6 THEN 'Sunday'
                END
                AND ff2.dayOfWeek = CASE DATEDIFF(DAY, '19000101', CAST(secondFlight.SecondDepartureDateTime AS DATE)) % 7
                    WHEN 0 THEN 'Monday'
                    WHEN 1 THEN 'Tuesday'
                    WHEN 2 THEN 'Wednesday'
                    WHEN 3 THEN 'Thursday'
                    WHEN 4 THEN 'Friday'
                    WHEN 5 THEN 'Saturday'
                    WHEN 6 THEN 'Sunday'
                END

                AND firstFlight.FirstDepartureDateTime >= @earliestDeparture
                AND firstFlight.FirstArrivalDateTime <= secondFlight.SecondDepartureDateTime
                AND DATEDIFF(HOUR, firstFlight.FirstArrivalDateTime, secondFlight.SecondDepartureDateTime) BETWEEN 1 AND 12
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

