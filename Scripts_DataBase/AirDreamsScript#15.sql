--- Indexes
USE AirDreams;
GO

CREATE NONCLUSTERED INDEX IX_Flight_DepartureDate_Route
ON Flight(departureDate, routeId)
INCLUDE (numberFlight);
GO

CREATE NONCLUSTERED INDEX IX_Route_Origen_Destino
ON Route(codeAirportSalida, codeAirportLlegada, idRoute)
INCLUDE (firstClassPrice, turistClassPrice, luggagePrice, carryOnPrice, porcentageMultiplier, modelo);
GO

CREATE NONCLUSTERED INDEX IX_Tiene_FlightNumber
ON Tiene(flightNumber, transactionId);
GO

CREATE NONCLUSTERED INDEX IX_Registra_Transaction
ON Registra(transactionIdItinerary, luggageNumber);
GO


CREATE OR ALTER PROCEDURE dbo.sp_GetIncomeReport
    @FechaInicio DATE,
    @FechaFinExclusiva DATE,
    @Origen VARCHAR(3) = NULL,
    @Destino VARCHAR(3) = NULL
    --- Falta aerolineas aun...
AS
BEGIN
    SET NOCOUNT ON;

    WITH FlightsFiltered AS (
        SELECT
            f.numberFlight,
            f.departureDate,
            f.occupiedFirstclass,
            f.occupiedTurist,
            r.idRoute,
            r.codeAirportSalida,
            r.codeAirportLlegada,
            r.firstClassPrice,
            r.turistClassPrice,
            r.luggagePrice,
            r.carryOnPrice,
            r.porcentageMultiplier
        FROM Flight f
        INNER JOIN Route r
            ON r.idRoute = f.routeId
        WHERE f.departureDate >= @FechaInicio
            AND f.departureDate < @FechaFinExclusiva
            AND (@Origen IS NULL OR r.codeAirportSalida = @Origen)
            AND (@Destino IS NULL OR r.codeAirportLlegada = @Destino)
    ),

    FlightMonthly AS (
        SELECT
            YEAR(departureDate) AS Año,
            MONTH(departureDate) AS MesNumero,
            COUNT(*) AS CantidadVuelos,
            SUM(occupiedFirstclass) AS TotalPasajerosPrimeraClase,
            SUM(occupiedTurist) AS TotalPasajerosClaseEconomica,
            SUM(occupiedFirstclass + occupiedTurist) AS TotalPasajeros,
            SUM(
                (occupiedFirstclass * firstClassPrice)
                +
                (occupiedTurist * turistClassPrice)
            ) AS IngresosTiquetes
        FROM FlightsFiltered
        GROUP BY
            YEAR(departureDate),
            MONTH(departureDate)
    ),

    LuggageByFlight AS (
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

    LuggageMonthly AS (
        SELECT
            YEAR(ff.departureDate) AS Año,
            MONTH(ff.departureDate) AS MesNumero,
            SUM(ISNULL(lbf.TotalMaletasDocumentadas, 0)) AS TotalMaletasDocumentadas,
            SUM(ISNULL(lbf.TotalMaletasCarryOn, 0)) AS TotalMaletasCarryOn,
            SUM(ISNULL(lbf.TotalMaletas, 0)) AS TotalMaletas,
            SUM(ISNULL(lbf.IngresosMaletas, 0)) AS IngresosMaletas
        FROM FlightsFiltered ff
        LEFT JOIN LuggageByFlight lbf
            ON lbf.flightNumber = ff.numberFlight
        GROUP BY
            YEAR(ff.departureDate),
            MONTH(ff.departureDate)
    )

    SELECT
        CONCAT(
            DATENAME(MONTH, DATEFROMPARTS(fm.Año, fm.MesNumero, 1)),
            ' ',
            fm.Año
        ) AS Mes,
        fm.Año,
        fm.MesNumero,
        fm.CantidadVuelos,
        fm.TotalPasajerosPrimeraClase,
        fm.TotalPasajerosClaseEconomica,
        fm.TotalPasajeros,
        fm.IngresosTiquetes,
        ISNULL(lm.TotalMaletasDocumentadas, 0) AS TotalMaletasDocumentadas,
        ISNULL(lm.TotalMaletasCarryOn, 0) AS TotalMaletasCarryOn,
        ISNULL(lm.TotalMaletas, 0) AS TotalMaletas,
        ISNULL(lm.IngresosMaletas, 0) AS IngresosMaletas,
        fm.IngresosTiquetes + ISNULL(lm.IngresosMaletas, 0) AS TotalIngresos
    FROM FlightMonthly fm
    LEFT JOIN LuggageMonthly lm
        ON lm.Año = fm.Año
        AND lm.MesNumero = fm.MesNumero
    ORDER BY
        fm.Año,
        fm.MesNumero;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetIncomeReportFilters
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT codeAirportSalida AS Code
    FROM Route
    ORDER BY codeAirportSalida;

    SELECT DISTINCT codeAirportLlegada AS Code
    FROM Route
    ORDER BY codeAirportLlegada;

    SELECT DISTINCT YEAR(departureDate) AS Year
    FROM Flight
    ORDER BY YEAR(departureDate) DESC;
END
GO


SELECT * FROM Flight