ALTER TABLE Itinerary
ADD seatClass VARCHAR(20) NULL;

ALTER TABLE Passenger ADD birthDate DATE NULL;
GO

IF OBJECT_ID('dbo.sp_InsertPassengersAndLuggage', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_InsertPassengersAndLuggage;
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
        INSERT INTO @PassengerMapping (PassengerIndex, IdPassenger)
        SELECT PassengerIndex, @CurrentMaxPassengerId + RowNumber
        FROM OrderedPassengers;

        INSERT INTO Passenger (idPassenger, namePassenger, lastnamesPassenger,
                               emailPassenger, telephone, country, birthDate)
        SELECT pm.IdPassenger, p.NamePassenger, p.LastnamesPassenger,
               p.EmailPassenger,
               TRY_CONVERT(BIGINT, NULLIF(p.Telephone, '')),
               p.Country,
               p.BirthDate
        FROM @Passengers p
        INNER JOIN @PassengerMapping pm ON pm.PassengerIndex = p.PassengerIndex;

        INSERT INTO Luggage (luggageNumber, type, quantity)
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