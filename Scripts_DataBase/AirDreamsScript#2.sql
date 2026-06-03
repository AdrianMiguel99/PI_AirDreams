USE AirDreams;
GO

DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql += '
ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) +
' DROP CONSTRAINT ' + QUOTENAME(fk.name) + ';'
FROM sys.foreign_keys fk
JOIN sys.tables t 
    ON fk.parent_object_id = t.object_id
WHERE fk.referenced_object_id = OBJECT_ID('Passenger');

EXEC sp_executesql @sql;
GO


DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql += '
ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) +
' DROP CONSTRAINT ' + QUOTENAME(kc.name) + ';'
FROM sys.key_constraints kc
JOIN sys.tables t 
    ON kc.parent_object_id = t.object_id
WHERE kc.type = 'PK'
AND t.name IN ('Passenger', 'Realiza', 'Registra');

EXEC sp_executesql @sql;
GO


DECLARE @sql NVARCHAR(MAX) = '';

    SELECT @sql += '
        ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) +
        ' DROP CONSTRAINT ' + QUOTENAME(cc.name) + ';'
    FROM sys.check_constraints cc
    JOIN sys.tables t 
            ON cc.parent_object_id = t.object_id
            WHERE cc.definition LIKE '%routeState%'
            OR cc.definition LIKE '%idPassenger%'
            OR cc.definition LIKE '%countryCode%';

EXEC sp_executesql @sql;
GO



IF COL_LENGTH('Route', 'routeState') IS NOT NULL
    ALTER TABLE Route DROP COLUMN routeState;
GO

IF COL_LENGTH('Route', 'luggagePrice') IS NULL
    ALTER TABLE Route ADD luggagePrice DECIMAL(5,3) NOT NULL DEFAULT 0;
GO

IF COL_LENGTH('Route', 'carryOnPrice') IS NULL
    ALTER TABLE Route ADD carryOnPrice DECIMAL(5,3) NOT NULL DEFAULT 0;
GO

IF COL_LENGTH('Route', 'carryOnMaxWeight') IS NULL
    ALTER TABLE Route ADD carryOnMaxWeight DECIMAL(4,2) DEFAULT 0;
GO

IF COL_LENGTH('Route', 'luggageMaxWeight') IS NULL
    ALTER TABLE Route ADD luggageMaxWeight DECIMAL(4,2) DEFAULT 0;
GO

IF COL_LENGTH('Route', 'porcentageMultiplier') IS NULL
    ALTER TABLE Route ADD porcentageMultiplier DECIMAL(5,2) DEFAULT 0.20;
GO



IF COL_LENGTH('Flight', 'flightState') IS NULL
BEGIN
    ALTER TABLE Flight
    ADD flightState VARCHAR(30) NOT NULL
    CONSTRAINT DF_Flight_flightState DEFAULT 'On-time'
    CONSTRAINT CK_Flight_flightState CHECK (
        flightState IN (
            'On-time',
            'Boarding',
            'Delayed',
            'Canceled',
            'In-Flight',
            'Landed'
        )
    );
END
GO

IF COL_LENGTH('Flight', 'priceLuggage') IS NOT NULL
    ALTER TABLE Flight DROP COLUMN priceLuggage;
GO


IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c
        ON dc.parent_object_id = c.object_id
        AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID('FlightFrequency')
        AND c.name = 'startingDate'
)
BEGIN
    ALTER TABLE FlightFrequency
    ADD CONSTRAINT DF_FlightFrequency_StartingDate
    DEFAULT CAST(GETDATE() AS DATE) FOR startingDate;
END
GO

IF COL_LENGTH('Itinerary', 'passportPassenger') IS NOT NULL
    ALTER TABLE Itinerary DROP COLUMN passportPassenger;
GO

IF COL_LENGTH('Realiza', 'passportPassenger') IS NOT NULL
    ALTER TABLE Realiza DROP COLUMN passportPassenger;
GO

IF COL_LENGTH('Registra', 'passportPassenger') IS NOT NULL
    ALTER TABLE Registra DROP COLUMN passportPassenger;
GO

IF COL_LENGTH('Passenger', 'passport') IS NOT NULL
    ALTER TABLE Passenger DROP COLUMN passport;
GO

IF COL_LENGTH('Passenger', 'countryCode') IS NOT NULL
    ALTER TABLE Passenger DROP COLUMN countryCode;
GO



IF COL_LENGTH('Passenger', 'country') IS NULL
    ALTER TABLE Passenger ADD country VARCHAR(50);
GO

IF COL_LENGTH('Passenger', 'emailPassenger') IS NOT NULL
    ALTER TABLE Passenger ALTER COLUMN emailPassenger VARCHAR(50) NULL;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints
    WHERE parent_object_id = OBJECT_ID('Passenger')
    AND type = 'PK'
)
BEGIN
    ALTER TABLE Passenger
    ADD CONSTRAINT PK_Passenger_idPassenger PRIMARY KEY (idPassenger);
END
GO


IF OBJECT_ID('PassengerItinerary', 'U') IS NULL
BEGIN
    CREATE TABLE PassengerItinerary (
        idPassenger INT NOT NULL,
        transactionId VARCHAR(20) NOT NULL,

        CONSTRAINT PK_PassengerItinerary
        PRIMARY KEY (idPassenger, transactionId),

        CONSTRAINT FK_PassengerItinerary_Passenger
        FOREIGN KEY (idPassenger)
        REFERENCES Passenger(idPassenger),

        CONSTRAINT FK_PassengerItinerary_Itinerary
        FOREIGN KEY (transactionId)
        REFERENCES Itinerary(transactionId)
    );
END
GO


SELECT * FROM Passenger;
SELECT * FROM PassengerItinerary;
SELECT * FROM Aircraft;
SELECT * FROM Route;
SELECT * FROM FlightFrequency;
GO