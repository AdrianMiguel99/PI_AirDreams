USE AirDreams;
GO

-- 1. Función de cálculo de equipaje
CREATE FUNCTION dbo.fn_TotalLuggageCost
(
    @basePrice DECIMAL(10,2),
    @multiplier DECIMAL(5,2),
    @quantity INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @total DECIMAL(10,2) = 0;
    IF @quantity <= 0 RETURN 0;
    IF @multiplier = 0
        SET @total = @basePrice * @quantity;
    ELSE
        SET @total = @basePrice * (POWER(1 + @multiplier, @quantity) - 1) / @multiplier;
    RETURN @total;
END;
GO

-- 2. Tipos de tabla personalizados 

CREATE TYPE dbo.PassengerPurchaseType AS TABLE (
    PassengerIndex INT NOT NULL,
    NamePassenger VARCHAR(100) NOT NULL,
    LastnamesPassenger VARCHAR(100) NOT NULL,
    EmailPassenger VARCHAR(50) NULL,
    Telephone VARCHAR(50) NULL,
    Country VARCHAR(100) NOT NULL
);
GO

CREATE TYPE dbo.LuggagePurchaseType AS TABLE (
    PassengerIndex INT NOT NULL,
    Type VARCHAR(50) NOT NULL,
    Quantity TINYINT NOT NULL
);
GO

CREATE TYPE dbo.FlightSegmentType AS TABLE (
    FlightNumber VARCHAR(50) NOT NULL,
    RouteId INT NOT NULL,
    CheckedPrice DECIMAL(10,2) NOT NULL,
    CarryOnPrice DECIMAL(10,2) NOT NULL,
    Multiplier DECIMAL(5,2) NOT NULL
);
GO

-- 3. Procedimientos almacenados

-- Insertar pasajeros y equipaje
CREATE PROCEDURE dbo.sp_InsertPassengersAndLuggage
    @TransactionId VARCHAR(20),
    @Passengers dbo.PassengerPurchaseType READONLY,
    @Luggage dbo.LuggagePurchaseType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @CurrentMaxPassengerId INT =
            ISNULL((SELECT MAX(idPassenger) FROM Passenger WITH (UPDLOCK, HOLDLOCK)), 0);

        DECLARE @PassengerMapping TABLE (
            PassengerIndex INT PRIMARY KEY,
            IdPassenger INT NOT NULL
        );

        ;WITH OrderedPassengers AS (
            SELECT PassengerIndex,
                   ROW_NUMBER() OVER (ORDER BY PassengerIndex) AS RowNumber
            FROM @Passengers
        )
        INSERT INTO @PassengerMapping (PassengerIndex, IdPassenger)
        SELECT PassengerIndex, @CurrentMaxPassengerId + RowNumber
        FROM OrderedPassengers;

        INSERT INTO Passenger (idPassenger, namePassenger, lastnamesPassenger,
                               emailPassenger, telephone, country)
        SELECT pm.IdPassenger, p.NamePassenger, p.LastnamesPassenger,
               p.EmailPassenger,
               TRY_CONVERT(BIGINT, NULLIF(p.Telephone, '')),
               p.Country
        FROM @Passengers p
        INNER JOIN @PassengerMapping pm ON pm.PassengerIndex = p.PassengerIndex;

        -- Equipaje
        INSERT INTO Luggage (luggageNumber, type, quantity)
        SELECT CONCAT('LUG-', @TransactionId, '-', PassengerIndex, '-', Type),
               Type, Quantity
        FROM @Luggage
        WHERE Quantity > 0;

        COMMIT TRANSACTION;

        -- Devolver los mapeos para que C# pueda insertar PassengerItinerary y Registra en lote
        SELECT PassengerIndex, IdPassenger FROM @PassengerMapping ORDER BY PassengerIndex;
        SELECT PassengerIndex,
               CONCAT('LUG-', @TransactionId, '-', PassengerIndex, '-', Type) AS LuggageNumber,
               Type, Quantity
        FROM @Luggage WHERE Quantity > 0
        ORDER BY PassengerIndex, Type;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Insertar segmentos de vuelo  y reservar asientos
CREATE PROCEDURE dbo.sp_InsertFlightSegments
    @TransactionId VARCHAR(20),
    @Segments dbo.FlightSegmentType READONLY,
    @SeatClass VARCHAR(20),
    @PassengerCount INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Crear vuelos que no existan
    INSERT INTO Flight (numberFlight, routeId, boardingGate, departureDate,
                        flightState, occupiedFirstclass, occupiedTurist)
    SELECT s.FlightNumber, s.RouteId, 1, CAST(GETDATE() AS DATE),
           'On-time', 0, 0
    FROM @Segments s
    WHERE NOT EXISTS (SELECT 1 FROM Flight f WHERE f.numberFlight = s.FlightNumber);

    -- Relacionar con itinerario
    INSERT INTO Tiene (transactionId, flightNumber)
    SELECT @TransactionId, s.FlightNumber
    FROM @Segments s;

    -- Reservar asientos (actualizar ocupación)
    IF @SeatClass = 'FirstClass'
    BEGIN
        UPDATE Flight
        SET occupiedFirstclass = occupiedFirstclass + @PassengerCount
        WHERE numberFlight IN (SELECT FlightNumber FROM @Segments);
    END
    ELSE IF @SeatClass = 'Turista'
    BEGIN
        UPDATE Flight
        SET occupiedTurist = occupiedTurist + @PassengerCount
        WHERE numberFlight IN (SELECT FlightNumber FROM @Segments);
    END
END
GO

-- Calcular costo total del equipaje en lote
CREATE OR ALTER PROCEDURE dbo.sp_CalculateLuggageCost
    @Segments dbo.FlightSegmentType READONLY,
    @Luggage dbo.LuggagePurchaseType READONLY,
    @TotalCost DECIMAL(10,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @TotalCost = ISNULL(SUM(
        dbo.fn_TotalLuggageCost(
            CASE WHEN l.Type = 'checked' THEN s.CheckedPrice ELSE s.CarryOnPrice END,
            s.Multiplier,
            l.Quantity
        )
    ), 0)   
    FROM @Luggage l
    CROSS JOIN @Segments s;
END
GO


