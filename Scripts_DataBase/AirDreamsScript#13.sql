USE AirDreams;
GO

IF OBJECT_ID('dbo.sp_CalculateLuggageCost', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_CalculateLuggageCost;
GO
IF OBJECT_ID('dbo.sp_InsertFlightSegments', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_InsertFlightSegments;
GO
IF OBJECT_ID('dbo.sp_InsertPassengersAndLuggage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_InsertPassengersAndLuggage;
GO
IF TYPE_ID('dbo.FlightSegmentType') IS NOT NULL
    DROP TYPE dbo.FlightSegmentType;
GO
IF TYPE_ID('dbo.FlightSegmentTypeV2') IS NOT NULL
    DROP TYPE dbo.FlightSegmentTypeV2;
GO
IF TYPE_ID('dbo.PassengerPurchaseType') IS NOT NULL
    DROP TYPE dbo.PassengerPurchaseType;
GO
IF TYPE_ID('dbo.LuggagePurchaseType') IS NOT NULL
    DROP TYPE dbo.LuggagePurchaseType;
GO
IF OBJECT_ID('dbo.fn_TotalLuggageCost', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_TotalLuggageCost;
GO


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

CREATE TYPE dbo.PassengerPurchaseType AS TABLE (
    PassengerIndex INT NOT NULL,
    NamePassenger VARCHAR(100) NOT NULL,
    LastnamesPassenger VARCHAR(100) NOT NULL,
    EmailPassenger VARCHAR(50) NULL,
    Telephone VARCHAR(50) NULL,
    Country VARCHAR(100) NOT NULL,
    BirthDate DATE NULL
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
    DepartureDate DATE NOT NULL,
    CheckedPrice DECIMAL(10,2) NOT NULL,
    CarryOnPrice DECIMAL(10,2) NOT NULL,
    Multiplier DECIMAL(5,2) NOT NULL
);
GO

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
        INSERT INTO @PassengerMapping (
            PassengerIndex,
            IdPassenger
        )
        SELECT PassengerIndex, @CurrentMaxPassengerId + RowNumber
        FROM OrderedPassengers;

        INSERT INTO Passenger (
            idPassenger,
            namePassenger,
            lastnamesPassenger,
            emailPassenger,
            telephone,
            country
        )
        SELECT pm.IdPassenger, p.NamePassenger, p.LastnamesPassenger,
            p.EmailPassenger,
            TRY_CONVERT(BIGINT, NULLIF(p.Telephone, '')),
            p.Country
        FROM @Passengers p
        INNER JOIN @PassengerMapping pm ON pm.PassengerIndex = p.PassengerIndex;

        INSERT INTO Luggage (luggageNumber,
        type,
        quantity
        )
        SELECT CONCAT('LUG-', @TransactionId, '-', PassengerIndex, '-', Type),
            Type, Quantity
        FROM @Luggage
        WHERE Quantity > 0;

        COMMIT TRANSACTION;

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

CREATE PROCEDURE dbo.sp_InsertFlightSegments
    @TransactionId VARCHAR(20),
    @Segments dbo.FlightSegmentType READONLY,
    @SeatClass VARCHAR(20),
    @PassengerCount INT
AS
BEGIN
    SET NOCOUNT ON;


    INSERT INTO Flight (
        numberFlight,
        routeId,
        boardingGate,
        departureDate,
        flightState,
        occupiedFirstclass,
        occupiedTurist
    )
    SELECT s.FlightNumber, s.RouteId, 1, s.DepartureDate,
        'On-time', 0, 0
    FROM @Segments s
    WHERE NOT EXISTS (SELECT 1 FROM Flight f WHERE f.numberFlight = s.FlightNumber);


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

DBCC FREEPROCCACHE;

GO


Select * from Flight