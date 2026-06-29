USE AirDreams;
GO

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('Airport') AND name = 'isActive')
BEGIN
    ALTER TABLE Airport ADD isActive BIT NOT NULL DEFAULT 1;
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'fk_route_airport_salida')
    ALTER TABLE Route DROP CONSTRAINT fk_route_airport_salida;
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'fk_route_airport_llegada')
    ALTER TABLE Route DROP CONSTRAINT fk_route_airport_llegada;
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'fk_flightfrequency_route')
    ALTER TABLE FlightFrequency DROP CONSTRAINT fk_flightfrequency_route;
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'fk_nombre')
    ALTER TABLE Flight DROP CONSTRAINT fk_nombre;
GO

ALTER TABLE Route
ADD CONSTRAINT fk_route_airport_salida
FOREIGN KEY (codeAirportSalida) REFERENCES Airport(codeAirport);

ALTER TABLE Route
ADD CONSTRAINT fk_route_airport_llegada
FOREIGN KEY (codeAirportLlegada) REFERENCES Airport(codeAirport);

ALTER TABLE FlightFrequency
ADD CONSTRAINT fk_flightfrequency_route
FOREIGN KEY (idRoute) REFERENCES Route(idRoute) ON DELETE CASCADE;

ALTER TABLE Flight
ADD CONSTRAINT fk_nombre
FOREIGN KEY (routeId) REFERENCES Route(idRoute) ON DELETE CASCADE;
GO

CREATE OR ALTER TRIGGER trg_Airport_Delete_Cascade
ON Airport
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Route
    WHERE codeAirportSalida IN (SELECT codeAirport FROM deleted)
       OR codeAirportLlegada IN (SELECT codeAirport FROM deleted);

    DELETE FROM Airport
    WHERE codeAirport IN (SELECT codeAirport FROM deleted);
END
GO
