Use AirDreams
GO

Alter Table Route
ALTER COLUMN carryOnMaxWeight DECIMAL(6, 2) NOT NULL;

Alter Table Route
ALTER COLUMN luggageMaxWeight DECIMAL(6, 2) NOT NULL;

Alter Table Flight
ADD occupiedLuggage DECIMAL(6, 2) NOT NULL DEFAULT 0;

Alter Table Flight
ADD occupiedCarryOn DECIMAL(6, 2) NOT NULL DEFAULT 0;
GO

CREATE FUNCTION dbo.CheckLuggageWeight (@flightId VARCHAR(15), @routeId INT, @luggageWeight DECIMAL(6,2))
RETURNS BIT
AS
BEGIN
    DECLARE @maxLuggageWeight DECIMAL(6,2);
    DECLARE @occupiedLuggage DECIMAL(6,2);

    SELECT
        @maxLuggageWeight = r.luggageMaxWeight,
        @occupiedLuggage = f.occupiedLuggage
    FROM Flight f
    INNER JOIN Route r
        ON r.idRoute = f.routeId
    WHERE f.numberFlight = @flightId;

    IF @maxLuggageWeight IS NULL
    BEGIN
        SELECT @maxLuggageWeight = luggageMaxWeight
        FROM Route
        WHERE idRoute = @routeId;

        IF @maxLuggageWeight IS NULL
            RETURN 0;

        IF @luggageWeight <= @maxLuggageWeight
            RETURN 1;

        RETURN 0;
    END

    IF (@occupiedLuggage + @luggageWeight) <= @maxLuggageWeight
        RETURN 1;

    RETURN 0;
END;
GO


CREATE FUNCTION dbo.CheckCarryOnWeight (@flightId VARCHAR(15), @routeId INT, @carryOnWeight DECIMAL(6,2))
RETURNS BIT
AS
BEGIN
    DECLARE @maxCarryOnWeight DECIMAL(6,2);
    DECLARE @occupiedCarryOn DECIMAL(6,2);

    SELECT
        @maxCarryOnWeight = r.carryOnMaxWeight,
        @occupiedCarryOn = f.occupiedCarryOn
    FROM Flight f
    INNER JOIN Route r
        ON r.idRoute = f.routeId
    WHERE f.numberFlight = @flightId;

    IF @maxCarryOnWeight IS NULL
    BEGIN
        SELECT @maxCarryOnWeight = carryOnMaxWeight
        FROM Route
        WHERE idRoute = @routeId;

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