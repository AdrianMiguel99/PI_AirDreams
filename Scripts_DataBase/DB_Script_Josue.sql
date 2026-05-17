use AirDreams
GO

Alter Table FlightFrequency
ADD Constraint DF_FlightFrequency_StartingDate
DEFAULT CAST(GETDATE() AS DATE) FOR startingDate;