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



Select * FROM Route;
Select * FROM FlightFrequency;


Alter table Route
ADD luggage_price DECIMAL(5,3) NOT NULL DEFAULT 0

Alter table Route
ADD carryOn_price DECIMAL(5,3) NOT NULL DEFAULT 0

Alter table Route
ADD carryOn_maxWeight DECIMAL(4,2) DEFAULT 0

Alter table Route
Add luggage_maxWeight DECIMAL(4,2) DEFAULT 0

Alter table Route
Add porcentage_multiplier DECIMAL(5,2) DEFAULT 0.20

Alter Table Flight
Drop COLUMN priceLuggage


SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Route';


EXEC sp_rename 'Route.luggage_price', 'luggagePrice', 'COLUMN';
EXEC sp_rename 'Route.luggage_maxWeight', 'luggageMaxWeight', 'COLUMN';
EXEC sp_rename 'Route.carryOn_price', 'carryOnPrice', 'COLUMN';
EXEC sp_rename 'Route.carryOn_maxWeight', 'carryOnMaxWeight', 'COLUMN';
EXEC sp_rename 'Route.porcentage_multiplier', 'porcentageMultiplier', 'COLUMN';


Create Table PassengerItinerary(
    idPassenger INT NOT NULL,
	passportPassenger VARCHAR(8) NOT NULL,
    transactionId VARCHAR(20),

    PRIMARY KEY (idPassenger, passportPassenger, transactionId),

    CONSTRAINT fk_Itinerary
    FOREIGN KEY (transactionId)
    REFERENCES Itinerary(transactionId),

    CONSTRAINT fk_Passenger
    FOREIGN KEY (idPassenger, passportPassenger)
    REFERENCES Passenger(idPassenger, passport)
);
GO


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


