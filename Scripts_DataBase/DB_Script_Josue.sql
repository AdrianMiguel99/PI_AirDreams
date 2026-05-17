use AirDreams
GO

Alter Table FlightFrequency
ADD Constraint DF_FlightFrequency_StartingDate
DEFAULT CAST(GETDATE() AS DATE) FOR startingDate;

Alter table Route
DROP CONSTRAINT CK__Route__routeStat__49C3F6B7;

Alter table Route
DROP COLUMN routeState;

Alter table Flight
ADD flightState VARCHAR(30) NOT NULL
CHECK (flightState IN ('On-time', 'Boarding', 'Delayed', 'Canceled', 'In-Flight', 'Landed'))
Default 'On-time';