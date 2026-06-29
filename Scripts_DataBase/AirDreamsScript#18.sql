Use AirDreams
GO

IF COL_LENGTH('Route', 'isDeleted') IS NULL
BEGIN
    ALTER TABLE Route
    ADD isDeleted BIT NOT NULL
        CONSTRAINT DF_Route_isDeleted DEFAULT 0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DeleteRoute
    @RouteId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.Route WITH (UPDLOCK, HOLDLOCK)
            WHERE idRoute = @RouteId
                AND isDeleted = 0
        )
        BEGIN
            THROW 50001, 'La ruta no existe o ya fue eliminada.', 1;
        END;

        IF EXISTS (
            SELECT 1
            FROM dbo.Flight WITH (UPDLOCK, HOLDLOCK)
            WHERE routeId = @RouteId
        )
        BEGIN
            UPDATE dbo.Route
            SET
                isDeleted = 1
            WHERE idRoute = @RouteId;

            UPDATE dbo.FlightFrequency
            SET
                active = 0
            WHERE idRoute = @RouteId;

            COMMIT TRANSACTION;

            SELECT
                'delete' AS DeleteType,
                'La ruta posee vuelos asociados y fue desactivada.' AS Message;

            RETURN;
        END;

        DELETE FROM dbo.FlightFrequency
        WHERE idRoute = @RouteId;

        DELETE FROM dbo.Route
        WHERE idRoute = @RouteId;

        COMMIT TRANSACTION;

        SELECT
            'delete' AS DeleteType,
            'La ruta fue eliminada permanentemente.' AS Message;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

Update Route
Set isDeleted = 0;
