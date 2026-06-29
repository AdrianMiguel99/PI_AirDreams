
USE AirDreams;
GO

IF COL_LENGTH('Itinerary', 'seatClass') IS NULL
BEGIN
    ALTER TABLE Itinerary
    ADD seatClass VARCHAR(20) NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Flight_DepartureDate_Route'
        AND object_id = OBJECT_ID('Flight')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Flight_DepartureDate_Route
    ON Flight(departureDate, routeId)
    INCLUDE (numberFlight);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Route_Origen_Destino'
        AND object_id = OBJECT_ID('Route')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Route_Origen_Destino
    ON Route(codeAirportSalida, codeAirportLlegada, idRoute)
    INCLUDE (firstClassPrice, turistClassPrice, luggagePrice, carryOnPrice, porcentageMultiplier, modelo);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Tiene_FlightNumber'
        AND object_id = OBJECT_ID('Tiene')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tiene_FlightNumber
    ON Tiene(flightNumber, transactionId);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Registra_Transaction'
        AND object_id = OBJECT_ID('Registra')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Registra_Transaction
    ON Registra(transactionIdItinerary, luggageNumber);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_External_PartnerName'
        AND object_id = OBJECT_ID('ExternalFlight')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_External_PartnerName
    ON ExternalFlight(partnerName, flightNumber);
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetIncomeReport
    @FechaInicio DATE,
    @FechaFinExclusiva DATE,
    @Origen VARCHAR(3) = NULL,
    @Destino VARCHAR(3) = NULL,
    @Aerolinea VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    WITH PassengerCountByTransaction AS (
        SELECT
            transactionId,
            COUNT(*) AS PassengerCount
        FROM PassengerItinerary
        GROUP BY transactionId
    ),

    InternalLuggageByFlight AS (
        SELECT
            t.flightNumber,
            SUM(CASE WHEN LOWER(l.type) = 'checked' THEN l.quantity ELSE 0 END) AS TotalMaletasDocumentadas,
            SUM(CASE WHEN LOWER(l.type) = 'carryon' THEN l.quantity ELSE 0 END) AS TotalMaletasCarryOn,
            SUM(l.quantity) AS TotalMaletas,
            SUM(
                CASE
                    WHEN LOWER(l.type) = 'checked'
                        THEN dbo.fn_TotalLuggageCost(r.luggagePrice, r.porcentageMultiplier, l.quantity)
                    WHEN LOWER(l.type) = 'carryon'
                        THEN dbo.fn_TotalLuggageCost(r.carryOnPrice, r.porcentageMultiplier, l.quantity)
                    ELSE 0
                END
            ) AS IngresosMaletas
        FROM Tiene t
        INNER JOIN Flight f
            ON f.numberFlight = t.flightNumber
        INNER JOIN Route r
            ON r.idRoute = f.routeId
        INNER JOIN Registra rg
            ON rg.transactionIdItinerary = t.transactionId
        INNER JOIN Luggage l
            ON l.luggageNumber = rg.luggageNumber
        GROUP BY
            t.flightNumber
    ),

    ExternalLuggageByTransactionFlight AS (
        SELECT
            te.transactionId,
            te.externalFlightNumber,
            SUM(CASE WHEN LOWER(l.type) = 'checked' THEN l.quantity ELSE 0 END) AS TotalMaletasDocumentadas,
            SUM(CASE WHEN LOWER(l.type) = 'carryon' THEN l.quantity ELSE 0 END) AS TotalMaletasCarryOn,
            SUM(l.quantity) AS TotalMaletas,
            SUM(
                CASE
                    WHEN LOWER(l.type) = 'checked'
                        THEN ex.checkedPrice * l.quantity
                    WHEN LOWER(l.type) = 'carryon'
                        THEN ex.carryOnPrice * l.quantity
                    ELSE 0
                END
            ) AS IngresosMaletas
        FROM TieneExternal te
        INNER JOIN ExternalFlight ex
            ON ex.flightNumber = te.externalFlightNumber
        INNER JOIN Registra rg
            ON rg.transactionIdItinerary = te.transactionId
        INNER JOIN Luggage l
            ON l.luggageNumber = rg.luggageNumber
        GROUP BY
            te.transactionId,
            te.externalFlightNumber
    ),

    SegmentSales AS (
        SELECT
            f.numberFlight AS FlightNumber,
            f.departureDate AS DepartureDate,
            CAST('AirDreams' AS VARCHAR(100)) AS Aerolinea,
            r.codeAirportSalida AS CodeAirportSalida,
            r.codeAirportLlegada AS CodeAirportLlegada,
            f.occupiedFirstclass AS PasajerosPrimeraClase,
            f.occupiedTurist AS PasajerosClaseEconomica,
            f.occupiedFirstclass + f.occupiedTurist AS TotalPasajeros,
            (f.occupiedFirstclass * r.firstClassPrice)
                + (f.occupiedTurist * r.turistClassPrice) AS IngresosTiquetes,
            ISNULL(ilf.TotalMaletasDocumentadas, 0) AS TotalMaletasDocumentadas,
            ISNULL(ilf.TotalMaletasCarryOn, 0) AS TotalMaletasCarryOn,
            ISNULL(ilf.TotalMaletas, 0) AS TotalMaletas,
            ISNULL(ilf.IngresosMaletas, 0) AS IngresosMaletas
        FROM Flight f
        INNER JOIN Route r
            ON r.idRoute = f.routeId
        LEFT JOIN InternalLuggageByFlight ilf
            ON ilf.flightNumber = f.numberFlight
        WHERE f.departureDate >= @FechaInicio
            AND f.departureDate < @FechaFinExclusiva
            AND (@Origen IS NULL OR r.codeAirportSalida = @Origen)
            AND (@Destino IS NULL OR r.codeAirportLlegada = @Destino)
            AND (@Aerolinea IS NULL OR @Aerolinea = 'AirDreams')
            AND NOT EXISTS (
                SELECT 1
                FROM ExternalFlight ex
                WHERE ex.flightNumber = f.numberFlight
            )

        UNION ALL

        SELECT
            ex.flightNumber AS FlightNumber,
            CAST(ex.departureDateTime AS DATE) AS DepartureDate,
            ex.partnerName AS Aerolinea,
            ex.departureAirportCode AS CodeAirportSalida,
            ex.arrivalAirportCode AS CodeAirportLlegada,
            CASE
                WHEN i.seatClass = 'FirstClass' THEN ISNULL(pct.PassengerCount, 0)
                ELSE 0
            END AS PasajerosPrimeraClase,
            CASE
                WHEN i.seatClass = 'FirstClass' THEN 0
                ELSE ISNULL(pct.PassengerCount, 0)
            END AS PasajerosClaseEconomica,
            ISNULL(pct.PassengerCount, 0) AS TotalPasajeros,
            CASE
                WHEN i.seatClass = 'FirstClass'
                    THEN ISNULL(pct.PassengerCount, 0) * ex.firstClassPrice
                ELSE ISNULL(pct.PassengerCount, 0) * ex.touristPrice
            END AS IngresosTiquetes,
            ISNULL(elf.TotalMaletasDocumentadas, 0) AS TotalMaletasDocumentadas,
            ISNULL(elf.TotalMaletasCarryOn, 0) AS TotalMaletasCarryOn,
            ISNULL(elf.TotalMaletas, 0) AS TotalMaletas,
            ISNULL(elf.IngresosMaletas, 0) AS IngresosMaletas
        FROM TieneExternal te
        INNER JOIN ExternalFlight ex
            ON ex.flightNumber = te.externalFlightNumber
        INNER JOIN Itinerary i
            ON i.transactionId = te.transactionId
        LEFT JOIN PassengerCountByTransaction pct
            ON pct.transactionId = te.transactionId
        LEFT JOIN ExternalLuggageByTransactionFlight elf
            ON elf.transactionId = te.transactionId
            AND elf.externalFlightNumber = te.externalFlightNumber
        WHERE ex.departureDateTime >= @FechaInicio
            AND ex.departureDateTime < @FechaFinExclusiva
            AND (@Origen IS NULL OR ex.departureAirportCode = @Origen)
            AND (@Destino IS NULL OR ex.arrivalAirportCode = @Destino)
            AND (@Aerolinea IS NULL OR ex.partnerName = @Aerolinea)
    ),

    MonthlySales AS (
        SELECT
            YEAR(DepartureDate) AS Año,
            MONTH(DepartureDate) AS MesNumero,
            COUNT(DISTINCT Aerolinea + '|' + FlightNumber) AS CantidadVuelos,
            SUM(PasajerosPrimeraClase) AS TotalPasajerosPrimeraClase,
            SUM(PasajerosClaseEconomica) AS TotalPasajerosClaseEconomica,
            SUM(TotalPasajeros) AS TotalPasajeros,
            SUM(IngresosTiquetes) AS IngresosTiquetes,
            SUM(TotalMaletasDocumentadas) AS TotalMaletasDocumentadas,
            SUM(TotalMaletasCarryOn) AS TotalMaletasCarryOn,
            SUM(TotalMaletas) AS TotalMaletas,
            SUM(IngresosMaletas) AS IngresosMaletas
        FROM SegmentSales
        GROUP BY
            YEAR(DepartureDate),
            MONTH(DepartureDate)
    )

    SELECT
        CONCAT(
            DATENAME(MONTH, DATEFROMPARTS(ms.Año, ms.MesNumero, 1)),
            ' ',
            ms.Año
        ) AS Mes,
        ms.Año,
        ms.MesNumero,
        ms.CantidadVuelos,
        ms.TotalPasajerosPrimeraClase,
        ms.TotalPasajerosClaseEconomica,
        ms.TotalPasajeros,
        ms.IngresosTiquetes,
        ms.TotalMaletasDocumentadas,
        ms.TotalMaletasCarryOn,
        ms.TotalMaletas,
        ms.IngresosMaletas,
        ms.IngresosTiquetes + ms.IngresosMaletas AS TotalIngresos
    FROM MonthlySales ms
    ORDER BY
        ms.Año,
        ms.MesNumero;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetIncomeReportFilters
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT codeAirportSalida AS Code
    FROM Route
    UNION
    SELECT DISTINCT departureAirportCode AS Code
    FROM ExternalFlight
    ORDER BY Code;

    SELECT DISTINCT codeAirportLlegada AS Code
    FROM Route
    UNION
    SELECT DISTINCT arrivalAirportCode AS Code
    FROM ExternalFlight
    ORDER BY Code;

    SELECT DISTINCT YEAR(departureDate) AS Year
    FROM Flight
    UNION
    SELECT DISTINCT YEAR(departureDateTime) AS Year
    FROM ExternalFlight
    ORDER BY Year DESC;

    SELECT DISTINCT Airline
    FROM (
        SELECT CAST('AirDreams' AS VARCHAR(100)) AS Airline
        UNION
        SELECT partnerName AS Airline
        FROM ExternalFlight
    ) Airlines
    ORDER BY Airline;
END
GO


SELECT * FROM Flight
