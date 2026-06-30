Select * from ExternalFlight
Select * from Route
Select * from Flight
ALTER TABLE ExternalFlight
ADD carryOnMaxWeight DECIMAL(7,2) NOT NULL DEFAULT 1000.00,   
    luggageMaxWeight DECIMAL(7,2) NOT NULL DEFAULT 2300.00,   
    porcentageMultiplier DECIMAL(5,2) NOT NULL DEFAULT 0.50;

CREATE OR ALTER FUNCTION dbo.CheckLuggageWeight
(
    @flightId VARCHAR(50),
    @routeId INT,
    @luggageWeight DECIMAL(6,2)
)
RETURNS BIT
AS
BEGIN
    DECLARE @maxLuggageWeight DECIMAL(7,2);
    DECLARE @occupiedLuggage DECIMAL(6,2);

    -- Buscar en vuelos propios (Flight + Route)
    SELECT @maxLuggageWeight = r.luggageMaxWeight,
           @occupiedLuggage = ISNULL(f.occupiedLuggage, 0)
    FROM Flight f
    INNER JOIN Route r ON r.idRoute = f.routeId
    WHERE f.numberFlight = @flightId;

    -- Si no se encontró en Flight, buscar en la ruta directamente
    IF @maxLuggageWeight IS NULL AND @routeId IS NOT NULL
    BEGIN
        SELECT @maxLuggageWeight = luggageMaxWeight,
               @occupiedLuggage = 0
        FROM Route
        WHERE idRoute = @routeId;
    END

    -- Si aún no se encuentra, buscar en ExternalFlight
    IF @maxLuggageWeight IS NULL
    BEGIN
        SELECT @maxLuggageWeight = luggageMaxWeight
        FROM ExternalFlight
        WHERE flightNumber = @flightId;

        IF @maxLuggageWeight IS NULL
            RETURN 0;

        IF @luggageWeight <= @maxLuggageWeight
            RETURN 1;
        RETURN 0;
    END

    -- Para vuelos propios, validar ocupación + nuevo peso
    IF (@occupiedLuggage + @luggageWeight) <= @maxLuggageWeight
        RETURN 1;

    RETURN 0;
END;
GO

CREATE OR ALTER FUNCTION dbo.CheckCarryOnWeight
(
    @flightId VARCHAR(50),
    @routeId INT,
    @carryOnWeight DECIMAL(6,2)
)
RETURNS BIT
AS
BEGIN
    DECLARE @maxCarryOnWeight DECIMAL(7,2);
    DECLARE @occupiedCarryOn DECIMAL(6,2);

    SELECT @maxCarryOnWeight = r.carryOnMaxWeight,
           @occupiedCarryOn = ISNULL(f.occupiedCarryOn, 0)
    FROM Flight f
    INNER JOIN Route r ON r.idRoute = f.routeId
    WHERE f.numberFlight = @flightId;

    IF @maxCarryOnWeight IS NULL AND @routeId IS NOT NULL
    BEGIN
        SELECT @maxCarryOnWeight = carryOnMaxWeight,
               @occupiedCarryOn = 0
        FROM Route
        WHERE idRoute = @routeId;
    END

    IF @maxCarryOnWeight IS NULL
    BEGIN
        SELECT @maxCarryOnWeight = carryOnMaxWeight
        FROM ExternalFlight
        WHERE flightNumber = @flightId;

        IF @maxCarryOnWeight IS NULL
            RETURN 0;

        IF @carryOnWeight <= @maxCarryOnWeight
            RETURN 1;
        RETURN 0;
    END

    IF (@occupiedCarryOn + @carryOnWeight) <= @maxCarryOnWeight
        RETURN 1;

    RETURN 0;
END;
GO