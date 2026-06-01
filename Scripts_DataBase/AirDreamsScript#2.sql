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

Alter table Route
ADD luggagePrice DECIMAL(5,3) NOT NULL DEFAULT 0

Alter table Route
ADD carryOnPrice DECIMAL(5,3) NOT NULL DEFAULT 0

Alter table Route
ADD carryOnMaxWeight DECIMAL(4,2) DEFAULT 0

Alter table Route
Add luggageMaxWeight DECIMAL(4,2) DEFAULT 0

Alter table Route
Add porcentageMultiplier DECIMAL(5,2) DEFAULT 0.20

Alter Table Flight
Drop COLUMN priceLuggage

Create Table PassengerItinerary(
    idPassenger INT NOT NULL,
    transactionId VARCHAR(20),

    PRIMARY KEY (idPassenger, transactionId),

    CONSTRAINT fk_Itinerary
    FOREIGN KEY (transactionId)
    REFERENCES Itinerary(transactionId),

    CONSTRAINT fk_Passenger
    FOREIGN KEY (idPassenger)
    REFERENCES Passenger(idPassenger)
);
GO

SELECT * FROM Passenger;

SELECT * FROM PassengerItinerary;


ALTER TABLE Itinerary 
DROP CONSTRAINT fk_itinerary_passenger;

Alter Table Itinerary
DROP COLUMN passportPassenger;

GO

ALTER TABLE Realiza
DROP CONSTRAINT PK__Realiza__9EAB2043FB96A356;

ALTER TABLE Realiza
DROP CONSTRAINT fk_realiza_passenger;

ALTER TABLE Realiza
DROP COLUMN passportPassenger;

ALTER TABLE Registra
DROP CONSTRAINT PK__Registra__89FAC960D0AB05A6;

ALTER TABLE Registra
DROP CONSTRAINT fk_registra_passenger;

ALTER TABLE Registra
DROP COLUMN passportPassenger;

ALTER TABLE PassengerItineray
DROP fk_Passenger

ALTER TABLE PassengerItineray
DROP COLUMN passportPassenger;

ALTER TABLE Passenger
DROP CONSTRAINT CK__Passenger__idPas__5EBF139D;

ALTER TABLE Passenger
DROP CONSTRAINT PK__Passenge__FC0F47D7EFB529AE;

ALTER TABLE Registra
DROP CONSTRAINT fk_registra_passenger;

ALTER TABLE Registra
DROP COLUMN passportPassenger;

SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Passenger';

EXEC sp_helpconstraint 'Passenger';

ALTER TABLE Passenger
DROP COLUMN passport

ALTER TABLE Passenger
DROP CONSTRAINT CK__Passenger__count__60A75C0F;

ALTER TABLE Passenger
DROP COLUMN countryCode;


ALTER TABLE Passenger
ADD country varchar(50);

ALTER TABLE Passenger
ADD CONSTRAINT PK_idPassenger PRIMARY KEY (idPassenger);


SELECT * FROM Aircraft;

SELECT * FROM Route;
SELECT * FROM FlightFrequency;