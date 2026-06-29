USE AirDreams
GO

CREATE TABLE ExternalFlight
(
    flightNumber varchar(50) PRIMARY KEY,
    partnerName varchar(100) NOT NULL,
    departureAirportCode varchar(3) NOT NULL,
    arrivalAirportCode varchar(3) NOT NULL,
    departureDateTime datetime NOT NULL,
    arrivalDateTime datetime NOT NULL,
    duration time NOT NULL,
    touristPrice decimal(10,2) NOT NULL,
    firstClassPrice decimal(10,2) NOT NULL,
    carryOnPrice decimal(10,2) NOT NULL,
    checkedPrice decimal(10,2) NOT NULL
);
GO

CREATE TABLE TieneExternal
(
    transactionId VARCHAR(20) NOT NULL,
    externalFlightNumber VARCHAR(50) NOT NULL,

    PRIMARY KEY(transactionId, externalFlightNumber),

    FOREIGN KEY(transactionId)
        REFERENCES Itinerary(transactionId),

    FOREIGN KEY(externalFlightNumber)
        REFERENCES ExternalFlight(flightNumber)
);
GO

ALTER TABLE Itinerary
ADD hasExternalFlight BIT NOT NULL CONSTRAINT DF_Itinerary_HasExternalFlight DEFAULT(0);

ALTER TABLE ExternalFlight
ADD CONSTRAINT FK_ExternalFlight_DepartureAirport
FOREIGN KEY(departureAirportCode)
REFERENCES Airport(codeAirport);

ALTER TABLE ExternalFlight
ADD CONSTRAINT FK_ExternalFlight_ArrivalAirport
FOREIGN KEY(arrivalAirportCode)
REFERENCES Airport(codeAirport);
GO