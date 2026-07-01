-- Indice
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_PassengerItinerary_TransactionId_Passenger'
      AND object_id = OBJECT_ID('PassengerItinerary')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_PassengerItinerary_TransactionId_Passenger
    ON PassengerItinerary(transactionId, idPassenger);
END;
GO

-- Transaction sobre actualizar maletas de una aeronave
CREATE OR ALTER PROCEDURE dbo.sp_RegisterExtraLuggage
    @TransactionId VARCHAR(20),
    @IdPassenger INT,
    @Type VARCHAR(20),
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF @Quantity <= 0
        BEGIN
            THROW 50001, 'La cantidad de maletas debe ser mayor a cero.', 1;
        END;

        IF NOT EXISTS (
            SELECT 1
            FROM Passenger
            WHERE idPassenger = @IdPassenger
        )
        BEGIN
            THROW 50002, 'El pasajero no existe.', 1;
        END;

        IF NOT EXISTS (
            SELECT 1
            FROM Itinerary
            WHERE transactionId = @TransactionId
        )
        BEGIN
            THROW 50003, 'La reserva no existe.', 1;
        END;

        DECLARE @LuggageNumber VARCHAR(100);

        SET @LuggageNumber = CONCAT(
            'LX-',
            @idPassenger,
            '-',
            LEFT(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 8)
        );

        INSERT INTO Luggage (
            luggageNumber,
            type,
            quantity
        )
        VALUES (
            @LuggageNumber,
            @Type,
            @Quantity
        );

        INSERT INTO Registra (
            idPassenger,
            transactionIdItinerary,
            luggageNumber
        )
        VALUES (
            @IdPassenger,
            @TransactionId,
            @LuggageNumber
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;
GO
