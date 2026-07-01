CREATE OR ALTER PROCEDURE dbo.sp_GetFlightsReport
    @Origin VARCHAR(3) = NULL,
    @Destination VARCHAR(3) = NULL,
    @SeatClass VARCHAR(20) = NULL,   
    @FromDate DATE = NULL,
    @ToDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        flightDate,
        origin,
        destination,
        numberFlight,
        airline,
        firstClassPassengers,
        touristPassengers,
        passengerRevenue,
        luggageRevenue,
        totalRevenue
    FROM
    (
        SELECT
            f.departureDate AS flightDate,
            r.codeAirportSalida AS origin,
            r.codeAirportLlegada AS destination,
            f.numberFlight,
            'AirDreams' AS airline,
            f.occupiedFirstclass AS firstClassPassengers,
            f.occupiedTurist AS touristPassengers,
            ISNULL(f.occupiedFirstclass, 0) * r.firstClassPrice +
            ISNULL(f.occupiedTurist, 0) * r.turistClassPrice AS passengerRevenue,
            ISNULL(lug.luggageRevenue, 0) AS luggageRevenue,
            ISNULL(f.occupiedFirstclass, 0) * r.firstClassPrice +
            ISNULL(f.occupiedTurist, 0) * r.turistClassPrice +
            ISNULL(lug.luggageRevenue, 0) AS totalRevenue
        FROM Flight f
        JOIN Route r ON f.routeId = r.idRoute
        LEFT JOIN (
            SELECT
                t.flightNumber,
                SUM(
                    CASE WHEN l.type = 'checked'
                        THEN dbo.fn_TotalLuggageCost(r2.luggagePrice, r2.porcentageMultiplier, l.quantity)
                        ELSE dbo.fn_TotalLuggageCost(r2.carryOnPrice, r2.porcentageMultiplier, l.quantity)
                    END
                ) AS luggageRevenue
            FROM Tiene t
            JOIN Registra rg ON t.transactionId = rg.transactionIdItinerary
            JOIN Luggage l ON rg.luggageNumber = l.luggageNumber
            JOIN Flight f2 ON t.flightNumber = f2.numberFlight
            JOIN Route r2 ON f2.routeId = r2.idRoute
            GROUP BY t.flightNumber
        ) lug ON f.numberFlight = lug.flightNumber
        WHERE (@Origin IS NULL OR r.codeAirportSalida = @Origin)
          AND (@Destination IS NULL OR r.codeAirportLlegada = @Destination)
          AND (@FromDate IS NULL OR f.departureDate >= @FromDate)
          AND (@ToDate IS NULL OR f.departureDate < DATEADD(DAY, 1, @ToDate))
          AND (@SeatClass IS NULL OR
               (@SeatClass = 'FirstClass' AND f.occupiedFirstclass > 0) OR
               (@SeatClass = 'Turista' AND f.occupiedTurist > 0))

        UNION ALL

        SELECT
            e.departureDateTime AS flightDate,
            e.departureAirportCode AS origin,
            e.arrivalAirportCode AS destination,
            e.flightNumber,
            e.partnerName AS airline,
            ISNULL(pax.firstClassCount, 0) AS firstClassPassengers,
            ISNULL(pax.touristCount, 0) AS touristPassengers,
            ISNULL(pax.passengerRevenue, 0) AS passengerRevenue,
            ISNULL(lug.luggageRevenue, 0) AS luggageRevenue,
            ISNULL(pax.passengerRevenue, 0) + ISNULL(lug.luggageRevenue, 0) AS totalRevenue
        FROM ExternalFlight e
        LEFT JOIN (
            SELECT
                te.externalFlightNumber,
                SUM(CASE WHEN i.seatClass = 'FirstClass' THEN 1 ELSE 0 END) AS firstClassCount,
                SUM(CASE WHEN i.seatClass = 'Turista' THEN 1 ELSE 0 END) AS touristCount,
                SUM(i.amount) AS passengerRevenue  
            FROM TieneExternal te
            JOIN Itinerary i ON i.transactionId = te.transactionId
            WHERE i.seatClass IS NOT NULL
            GROUP BY te.externalFlightNumber
        ) pax ON pax.externalFlightNumber = e.flightNumber
        LEFT JOIN (
            SELECT
                te.externalFlightNumber,
                SUM(
                    CASE WHEN l.type = 'checked'
                        THEN dbo.fn_TotalLuggageCost(e2.checkedPrice, e2.porcentageMultiplier, l.quantity)
                        ELSE dbo.fn_TotalLuggageCost(e2.carryOnPrice, e2.porcentageMultiplier, l.quantity)
                    END
                ) AS luggageRevenue
            FROM TieneExternal te
            JOIN Registra rg ON te.transactionId = rg.transactionIdItinerary
            JOIN Luggage l ON rg.luggageNumber = l.luggageNumber
            JOIN ExternalFlight e2 ON te.externalFlightNumber = e2.flightNumber
            GROUP BY te.externalFlightNumber
        ) lug ON lug.externalFlightNumber = e.flightNumber
        WHERE (@Origin IS NULL OR e.departureAirportCode = @Origin)
          AND (@Destination IS NULL OR e.arrivalAirportCode = @Destination)
          AND (@FromDate IS NULL OR e.departureDateTime >= @FromDate)
          AND (@ToDate IS NULL OR e.departureDateTime < DATEADD(DAY, 1, @ToDate))
    ) AS ReportData
    ORDER BY flightDate, origin, destination;
END
GO

CREATE NONCLUSTERED INDEX IX_ExternalFlight_Date_Airports
ON ExternalFlight(departureDateTime, departureAirportCode, arrivalAirportCode)
INCLUDE (partnerName, firstClassPrice, touristPrice, carryOnPrice, checkedPrice);