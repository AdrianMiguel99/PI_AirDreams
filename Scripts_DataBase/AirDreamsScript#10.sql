USE AirDreams
GO

--Primero vamos a crear las tablas de campos ocupados en first y turist
ALTER TABLE FLIGHT
ADD occupiedFirstclass INT NOT NULL DEFAULT 0,
    occupiedTurist INT NOT NULL DEFAULT 0;
GO

-- Creamos un SP que si el campo esta ocupado retorna 1 si no 0.
USE AirDreams;
GO

CREATE OR ALTER PROCEDURE CheckFlightAvailability
    @NumberFlight NVARCHAR(50),
    @SeatClass NVARCHAR(20),
    @RequestedSeats INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM Flight
        WHERE numberFlight = @NumberFlight
    )
    BEGIN
        SELECT 1 AS IsAvailable;
        RETURN;
    END;

    SELECT
        CASE
            WHEN @SeatClass = 'FirstClass'
                AND f.occupiedFirstclass + @RequestedSeats <=
                     (a.cant_Asientos_Fila_Firstclass * a.cant_Filas_Firstclass)
            THEN 1

            WHEN @SeatClass = 'Turist'
                AND f.occupiedTurist + @RequestedSeats <=
                     (a.cant_Asientos_Fila_Turista * a.cant_Filas_Turista)
            THEN 1

            ELSE 0
        END AS IsAvailable
    FROM Flight f
    INNER JOIN Route r ON f.routeId = r.idRoute
    INNER JOIN Aircraft a ON r.modelo = a.modelo
    WHERE f.numberFlight = @NumberFlight;
END;
GO

-- Este SP va a ser muy parecido al anterior, su diferencia es que si modifica la tabla de vuelos, en caso de que si haya cupo sumaria la cantidad de cupos a la variable.
CREATE OR ALTER PROCEDURE ReserveFlightSeats
    @NumberFlight NVARCHAR(20),
    @SeatClass NVARCHAR(20),
    @RequestedSeats INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @SeatClass = 'FirstClass'
    BEGIN
        UPDATE Flight
        SET occupiedFirstclass = occupiedFirstclass + @RequestedSeats
        WHERE numberFlight = @NumberFlight;
    END
    ELSE IF @SeatClass = 'Turist'
    BEGIN
        UPDATE Flight
        SET occupiedTurist = occupiedTurist + @RequestedSeats
        WHERE numberFlight = @NumberFlight;
    END
END;
GO
