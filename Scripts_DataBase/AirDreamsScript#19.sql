USE AirDreams
GO

CREATE TABLE ExternalFlight
(
    flightNumber varchar(50) PRIMARY KEY,
    partnerName varchar(100) NOT NULL,
    departureAirportCode varchar(3) NOT NULL,
    arrivalAirportCode varchar(3) NOT NULL,
    departureDateTime datetime NOT NULL,
    arrivalDateTime datetime NOT NULL,
    duration time NOT NULL,
    touristPrice decimal(10,2) NOT NULL,
    firstClassPrice decimal(10,2) NOT NULL,
    carryOnPrice decimal(10,2) NOT NULL,
    checkedPrice decimal(10,2) NOT NULL,
    occupiedLuggage DECIMAL(6, 2) NOT NULL DEFAULT 0,
    occupiedCarryOn DECIMAL(6, 2) NOT NULL DEFAULT 0
);
GO

CREATE TABLE TieneExternal
(
    transactionId VARCHAR(20) NOT NULL,
    externalFlightNumber VARCHAR(50) NOT NULL,

    PRIMARY KEY(transactionId, externalFlightNumber),

    FOREIGN KEY(transactionId)
        REFERENCES Itinerary(transactionId),

    FOREIGN KEY(externalFlightNumber)
        REFERENCES ExternalFlight(flightNumber)
);
GO

ALTER TABLE Itinerary
ADD hasExternalFlight BIT NOT NULL CONSTRAINT DF_Itinerary_HasExternalFlight DEFAULT(0);

ALTER TABLE ExternalFlight
ADD CONSTRAINT FK_ExternalFlight_DepartureAirport
FOREIGN KEY(departureAirportCode)
REFERENCES Airport(codeAirport);

ALTER TABLE ExternalFlight
ADD CONSTRAINT FK_ExternalFlight_ArrivalAirport
FOREIGN KEY(arrivalAirportCode)
REFERENCES Airport(codeAirport);
GO

CREATE NONCLUSTERED INDEX IX_Registra
ON Registra(transactionIdItinerary);
GO

CREATE PROCEDURE UpdateItineraryFlightWeight
    @transactionId  VARCHAR(20),
    @luggageWeight  DECIMAL(6,2),
    @carryOnWeight  DECIMAL(6,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

    BEGIN TRANSACTION;

    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Tiene WHERE transactionId = @transactionId)
        BEGIN
            UPDATE f
            SET
                occupiedLuggage = occupiedLuggage + @luggageWeight,
                occupiedCarryOn = occupiedCarryOn + @carryOnWeight
            FROM Flight f
            INNER JOIN Tiene t ON t.flightNumber = f.numberFlight
            WHERE t.transactionId = @transactionId;
        END

        IF EXISTS (SELECT 1 FROM TieneExternal WHERE transactionId = @transactionId)
        BEGIN
            UPDATE ef
            SET
                occupiedLuggage = occupiedLuggage + @luggageWeight,
                occupiedCarryOn = occupiedCarryOn + @carryOnWeight
            FROM ExternalFlight ef
            INNER JOIN TieneExternal te ON te.externalFlightNumber = ef.flightNumber
            WHERE te.transactionId = @transactionId;
        END

        IF NOT EXISTS (SELECT 1 FROM Tiene WHERE transactionId = @transactionId)
            AND NOT EXISTS (SELECT 1 FROM TieneExternal WHERE transactionId = @transactionId)
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN -1;
        END

        COMMIT TRANSACTION;
        RETURN 1;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        RETURN -1;
    END CATCH
END
GO