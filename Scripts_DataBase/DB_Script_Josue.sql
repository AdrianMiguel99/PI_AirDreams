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

9

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