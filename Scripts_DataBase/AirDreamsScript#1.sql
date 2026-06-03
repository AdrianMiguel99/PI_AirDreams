CREATE DATABASE AirDreams
GO

--TODO: MEJORAR VERIFICACIONES DE TAMANOS
--TODO: IMPLEMENTAR ON UPDATE ON DELETE ETC.

use AirDreams
GO


CREATE TABLE SystemUser(
	userID TINYINT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
);
GO


CREATE TABLE ExternalUser(
	airlineName varchar(40) NOT NULL,
	airlineEmail varchar(40) NOT NULL,
	--fk
	userID TINYINT NOT NULL,
	--fin fk
	secretKey varchar(256) NOT NULL,

	PRIMARY KEY (airlineName, airlineEmail),

	CONSTRAINT fk_externalUser
    FOREIGN KEY (userID)
    REFERENCES SystemUser(userID),
	
);
GO


CREATE TABLE InternalUser(
	emailUser varchar(40) PRIMARY KEY NOT NULL,
	--fk
	userID TINYINT NOT NULL,
	-- fin fk
	hashPasswordUser varchar(255) NOT NULL,
	isActive BIT NOT NULL,
	invitationToken nvarchar(255) NULL,
	invitationExpiryDate DATETIME,


	CONSTRAINT fk_internalUser
    FOREIGN KEY (userID)
    REFERENCES SystemUser(userID),

);
GO


CREATE TABLE AirlineEmployee(
	employeeID TINYINT PRIMARY KEY NOT NULL,
	--fk
	emailInternalUser varchar(40) NOT NULL,
	--fin fk
	lastnames varchar(15) NOT NULL,
	nameEmployee varchar(15) NOT NULL,
	isAdmin BIT NOT NULL,
	isOperator BIT NOT NULL,

	CONSTRAINT fk_airlineemployee
    FOREIGN KEY (emailInternalUser)
    REFERENCES InternalUser(emailUser),
);
GO


CREATE TABLE Aircraft (
	plateNumber varchar(15) Primary Key NOT NULL,
	adminID TINYINT NOT NULL,
	maxWeight INT NOT NULL,
	cantPasajeros INT Check(cantPasajeros<=1000) NOT NULL,
	cant_Asientos_Fila_Firstclass INT NOT NULL,
	cant_Filas_Firstclass INT NOT NULL,
	cant_Asientos_Fila_Turista INT NOT NULL,
	cant_Filas_Turista INT NOT NULL,
	--(alfanumerico solo admite -)
	modelo varchar(30) UNIQUE NOT NULL,
	

	CONSTRAINT fk_aircraft
    FOREIGN KEY (adminID)
    REFERENCES AirlineEmployee(employeeID),
);
GO


CREATE TABLE Airport(
	codeAirport varchar(3) PRIMARY KEY NOT NULL,
	--fk
	adminID TINYINT NOT NULL,
	--fin fk
	nameAirport varchar(200) NOT NULL,
	city varchar(50) NOT NULL,
	country varchar(56) NOT NULL,
	timeZone varchar(10),

	CONSTRAINT fk_adminid
    FOREIGN KEY (adminID)
    REFERENCES AirlineEmployee(employeeID)
);
GO


CREATE TABLE Route (
    idRoute  INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    adminID TINYINT NOT NULL,
    codeAirportSalida VARCHAR(3) NOT NULL,
    codeAirportLlegada VARCHAR(3) NOT NULL,
    plateNumber VARCHAR(15) NOT NULL,
    firstClassPrice DECIMAL(10,2) NOT NULL,
    turistClassPrice DECIMAL(10,2) NOT NULL,
    routeState VARCHAR(30) NOT NULL
        CHECK (routeState IN ('On-time', 'Boarding', 'Delayed', 'Canceled', 'In-Flight', 'Landed')),
    stimatedTime TIME,
    distance DECIMAL(10,2) NOT NULL,

    CONSTRAINT fk_route_admin
        FOREIGN KEY (adminID)
        REFERENCES AirlineEmployee(employeeID),

    CONSTRAINT fk_route_airport_salida
        FOREIGN KEY (codeAirportSalida)
        REFERENCES Airport(codeAirport),

    CONSTRAINT fk_route_airport_llegada
        FOREIGN KEY (codeAirportLlegada)
        REFERENCES Airport(codeAirport),

    CONSTRAINT fk_route_aircraft
        FOREIGN KEY (plateNumber)
        REFERENCES Aircraft(plateNumber)
);
GO

CREATE TABLE FlightFrequency(
	idFrequency INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	idRoute INT NOT NULL,
	dayOfWeek VARCHAR(15) NOT NULL,
	departureTime TIME NOT NULL,
	estimatedArrivalTime TIME NOT NULL,
	startingDate DATE NOT NULL,
	endingDate DATE NOT NULL,
	active BIT NOT NULL,

	CONSTRAINT chk_flightfrequency_day
        CHECK (dayOfWeek IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday')),

    CONSTRAINT chk_flightfrequency_dates
        CHECK (endingDate >= startingDate),

	CONSTRAINT fk_flightfrequency_route
		FOREIGN KEY (idRoute)
		REFERENCES Route(idRoute)
);
GO


CREATE TABLE Flight(
	numberFlight varchar(6) PRIMARY KEY NOT NULL,
	--fk
	routeId int NOT NULL,
	-- fin fk
	boardingGate TINYINT NOT NULL,
	priceLuggage DECIMAL(6,3),

	CONSTRAINT fk_nombre
    FOREIGN KEY (routeId)
    REFERENCES Route(idRoute)
	
);
GO

ALTER TABLE Flight
ADD departureDate DATE NOT NULL;


CREATE TABLE CheckIn(
	checkInNumber varchar(30) PRIMARY KEY NOT NULL,
	--fk
	operatorId TINYINT NOT NULL,
	flightNumber varchar(6) NOT NULL,
	--final fk
	finalPrice DECIMAL(10,2) NOT NULL,

	CONSTRAINT fk_checkin_operator
	FOREIGN KEY (operatorId)
    REFERENCES AirlineEmployee(employeeID),

    CONSTRAINT fk_checkin_flight
    FOREIGN KEY (flightNumber)
	REFERENCES Flight(numberFlight),
);
GO


CREATE TABLE BoardingPass(
	boardingCode varchar(30) PRIMARY KEY NOT NULL,
	--fk
	checkInNumber varchar(30) NOT NULL,
	-- final fk
	creationTime TIME NOT NULL,
	boardingTime TIME NOT NULL,

	qrCode varchar(150) NOT NULL,
	seatNumber INT NOT NULL,

	CONSTRAINT fk_boardingpass_checkin
    FOREIGN KEY (checkInNumber)
    REFERENCES CheckIn(checkInNumber),
);
GO



CREATE TABLE Passenger(
	idPassenger INT NOT NULL CHECK(idPassenger <= 99999),
	passport VARCHAR(8) NOT NULL,
	namePassenger VARCHAR(30) NOT NULL,
	lastnamesPassenger VARCHAR(30) NOT NULL,
	emailPassenger VARCHAR(50) NOT NULL,
	telephone BIGINT CHECK(telephone <= 999999999999999),
	countryCode TINYINT CHECK(countryCode <= 99999),

	PRIMARY KEY (idPassenger, passport)
);
GO


CREATE TABLE Itinerary(
	transactionId VARCHAR(20) PRIMARY KEY NOT NULL,

	-- fk compuesta
	idPassenger INT NOT NULL,
	passportPassenger VARCHAR(8) NOT NULL,
	-- fin fk

	purchaseDate DATE,
	amount DECIMAL(10,2),

	CONSTRAINT fk_itinerary_passenger
    FOREIGN KEY (idPassenger, passportPassenger)
    REFERENCES Passenger(idPassenger, passport)
);
GO

CREATE TABLE Realiza(
	idPassenger INT NOT NULL,
	passportPassenger VARCHAR(8) NOT NULL,
	checkInNumber VARCHAR(30) NOT NULL,

	PRIMARY KEY (idPassenger, passportPassenger, checkInNumber),

	CONSTRAINT fk_realiza_passenger
    FOREIGN KEY (idPassenger, passportPassenger)
    REFERENCES Passenger(idPassenger, passport),

	CONSTRAINT fk_realiza_checkin
    FOREIGN KEY (checkInNumber)
    REFERENCES CheckIn(checkInNumber)
);
GO

CREATE TABLE Tiene(
	-- pk, fk
	transactionId VARCHAR(20) NOT NULL,
	flightNumber VARCHAR(6) NOT NULL,
	-- final pk, fk

	PRIMARY KEY (transactionId, flightNumber),

	CONSTRAINT fk_tiene_itinerary
    FOREIGN KEY (transactionId)
    REFERENCES Itinerary(transactionId),

	CONSTRAINT fk_tiene_flight
    FOREIGN KEY (flightNumber)
    REFERENCES Flight(numberFlight)
);
GO


CREATE TABLE Luggage(
	luggageNumber VARCHAR(30) PRIMARY KEY NOT NULL,
	type VARCHAR(30),
	quantity TINYINT
);
GO

CREATE TABLE Registra(
	-- pk, fk
	idPassenger INT NOT NULL,
	passportPassenger VARCHAR(8) NOT NULL,
	transactionIdItinerary VARCHAR(20) NOT NULL,
	luggageNumber VARCHAR(30) NOT NULL,
	-- final pk, fk

	PRIMARY KEY (idPassenger, passportPassenger, transactionIdItinerary, luggageNumber),

	CONSTRAINT fk_registra_passenger
    FOREIGN KEY (idPassenger, passportPassenger)
    REFERENCES Passenger(idPassenger, passport),

	CONSTRAINT fk_registra_itinerary
    FOREIGN KEY (transactionIdItinerary)
    REFERENCES Itinerary(transactionId),

	CONSTRAINT fk_registra_luggage
    FOREIGN KEY (luggageNumber)
    REFERENCES Luggage(luggageNumber)
);
GO

CREATE TABLE Country(
	countryNumber TINYINT PRIMARY KEY NOT NULL,
	nameCountry VARCHAR(100) NOT NULL
);
GO


CREATE TABLE City(
	zipCode VARCHAR(10) PRIMARY KEY NOT NULL,
	--fk a Country--
	countryNumber TINYINT NOT NULL,
	--fin fk
	nameCity VARCHAR(100) NOT NULL,

	CONSTRAINT fk_country_number
    FOREIGN KEY (countryNumber)
    REFERENCES Country(countryNumber)
);
GO


USE AirDreams;
GO

-- 1. Crear SystemUser
INSERT INTO SystemUser DEFAULT VALUES;
GO

-- 2. Obtener el último userID generado
DECLARE @NewUserID TINYINT;
SET @NewUserID = SCOPE_IDENTITY();

-- 3. Crear InternalUser
INSERT INTO InternalUser (
    emailUser,
    userID,
    hashPasswordUser,
    isActive,
    invitationToken,
    invitationExpiryDate
)
VALUES (
    'admin3@air.com',
    @NewUserID,
    'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=',
    1,
    NULL,
    NULL
);

-- 4. Crear AirlineEmployee
INSERT INTO AirlineEmployee (
    employeeID,
    emailInternalUser,
    lastnames,
    nameEmployee,
    isAdmin,
    isOperator
)
VALUES (
    @NewUserID,
    'admin3@air.com',
    'Rodriguez',
    'Carlos',
    1,
    0
);
GO

INSERT INTO SystemUser DEFAULT VALUES;
INSERT INTO SystemUser DEFAULT VALUES;
INSERT INTO SystemUser DEFAULT VALUES;

INSERT INTO ExternalUser (
    airlineName,
    airlineEmail,
    userID,
    secretKey
)

VALUES (
    'Snoopy Airlines',
    'snoopyAirline@gmail.com',
    2,
    'TfWcIHIKq7n4Rl8ahULmiXac3SnSrx6yy7tJM4374sObLvxDV0e7Hu50FH9qz7k04pGzrSxGbTQnR9iq4S3Z6o0WuKK189X9BlQdwfq3cqFIFc9HhjlM27staPwWbxbjiwNV1gv9zSVmvnIGzKZQL86PEr9zZD25896opxwEwvGbLucvlXeFK2Z4XuVaDbYul0fIngvu7k6Hk92Td4EbPDtykOMPkWlsJMVKq1BL1OWtlmfB9nhHXTWlagvdICjl'
),
(
    'Mushu Airlines',
    'mushuAirline@gmail.com',
    3,
    'IDgmiRR3QxZGHFH0xjjksUkcSZMQqFZI8ASsdy2X0mfN3ZYnZulYLqcsvrhjfmRma29N5WxKtmb1ixTqbtQ76NlmoliVTWwA0zHB74OjxjfpOIogZfMMpBW4waUw8aWSpvF6VU3JSqvCuvt7qriEEhFMeC1Vjp7DRfB0a1YupwU7HVaN9QBw0O4XbbQJb2MX02y4DOqnB0kyExnAgB9yIm69AAlGDwW573fd0Li2JDcnXVHPi1smW5O4S2IcuhkZ'
),
(
    'Zuli Airlines',
    'zuliAirline@gmail.com',
    4,
    'SDIiE8dzYJSdyI6Uxi2n3GgJ5sgnNUlJxe07EvbiqHoO2FYz0mllpyF0dK3TJN4nCmJHmD4lv0MQgb2BM0qA2pPBF0vdOnWE1hnkiBqvi3uNXjIdz1KVGZ25ZXU8XwWlu0Uzu88JmUVGE6BW64fywMG4oNe9mECdcHdI3zXTUcqTkVJP9KiPbWGw1g71JK3EL9V0qs7R2veM8jk0ZHLfFRllt80J0G26OAq6oiB7W3FWP8NT84L7CMJyKTnv6r2o'
);
GO
