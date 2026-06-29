USE AirDreams;
GO

IF COL_LENGTH('Aircraft', 'isDeleted') IS NULL
BEGIN
    ALTER TABLE Aircraft
    ADD isDeleted BIT NOT NULL
        CONSTRAINT DF_Aircraft_isDeleted DEFAULT 0;
END;
GO


IF COL_LENGTH('Route', 'isDeleted') IS NULL
BEGIN
    ALTER TABLE Route
    ADD isDeleted BIT NOT NULL
        CONSTRAINT DF_Route_isDeleted DEFAULT 0;
END;
GO

CREATE OR ALTER VIEW AircraftView
AS
SELECT
    modelo,
    aircraftSize,
    maxWeight,

    cant_Asientos_Fila_Firstclass,
    cant_Filas_Firstclass,

    cant_Asientos_Fila_Turista,
    cant_Filas_Turista,

    dbo.CalcularPasajerosAeronave(
        cant_Asientos_Fila_Firstclass,
        cant_Filas_Firstclass,
        cant_Asientos_Fila_Turista,
        cant_Filas_Turista
    ) AS cantPasajeros
FROM Aircraft
WHERE isDeleted = 0;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DeleteAircraft
    @Modelo VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Aircraft WITH (UPDLOCK, HOLDLOCK)
            WHERE modelo = @Modelo
                AND isDeleted = 0
        )
        BEGIN
            THROW 50001, 'La aeronave no existe o ya fue eliminada.', 1;
        END;

        DECLARE @Routes TABLE
        (
            idRoute INT PRIMARY KEY
        );

        INSERT INTO @Routes (idRoute)
        SELECT idRoute
        FROM dbo.Route WITH (UPDLOCK, HOLDLOCK)
        WHERE modelo = @Modelo;

        IF NOT EXISTS (SELECT 1 FROM @Routes)
        BEGIN
            DELETE FROM dbo.Aircraft
            WHERE modelo = @Modelo;

            COMMIT TRANSACTION;

            SELECT
                'La aeronave fue eliminada permanentemente.' AS Message;

            RETURN;
        END;

        IF EXISTS (
            SELECT 1
            FROM dbo.Flight f WITH (UPDLOCK, HOLDLOCK)
            INNER JOIN @Routes r ON f.routeId = r.idRoute
        )
        BEGIN
            UPDATE dbo.Aircraft
            SET
                isDeleted = 1
            WHERE modelo = @Modelo;

            UPDATE dbo.Route
            SET
                isDeleted = 1
            WHERE modelo = @Modelo
                AND isDeleted = 0;

            UPDATE dbo.FlightFrequency
            SET active = 0
            WHERE idRoute IN (SELECT idRoute FROM @Routes);

            COMMIT TRANSACTION;

            SELECT
                'La aeronave posee rutas con vuelos asociados y fue desactivada.' AS Message;

            RETURN;
        END;

        DELETE FROM dbo.FlightFrequency
        WHERE idRoute IN (SELECT idRoute FROM @Routes);

        DELETE FROM dbo.Route
        WHERE idRoute IN (SELECT idRoute FROM @Routes);

        DELETE FROM dbo.Aircraft
        WHERE modelo = @Modelo;

        COMMIT TRANSACTION;

        SELECT
            'La aeronave y sus rutas fueron eliminadas permanentemente.' AS Message;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO


SELECT * FROM Flight

SELECT * FROM Route

SELECT * FROM Aircraft

