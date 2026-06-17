USE AirDreams;
GO

DELETE FROM PassengerItinerary;
DELETE FROM Registra;
DELETE FROM Realiza;
DELETE FROM Tiene;
DELETE FROM BoardingPass;
DELETE FROM CheckIn;
DELETE FROM Flight;
DELETE FROM FlightFrequency;
DELETE FROM Route;
DELETE FROM Itinerary;
DELETE FROM Passenger;
DELETE FROM Luggage;
DELETE FROM Aircraft;
DELETE FROM Airport;
DELETE FROM City;
DELETE FROM Country;
DELETE FROM AirlineEmployee;
DELETE FROM InternalUser;
DELETE FROM ExternalUser;
DELETE FROM SystemUser;

DBCC CHECKIDENT ('SystemUser', RESEED, 0);
DBCC CHECKIDENT ('Route', RESEED, 0);
DBCC CHECKIDENT ('FlightFrequency', RESEED, 0);

GO

INSERT INTO SystemUser DEFAULT VALUES;

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
    1,
    'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=',
    1,
    NULL,
    NULL
);

INSERT INTO AirlineEmployee (
    employeeID,
    emailInternalUser,
    lastnames,
    nameEmployee,
    isAdmin,
    isOperator
)
VALUES (
    1,
    'admin3@air.com',
    'Rodriguez',
    'Carlos',
    1,
    0
);

INSERT INTO Aircraft (adminID, cant_Asientos_Fila_Firstclass, cant_Filas_Firstclass, cant_Asientos_Fila_Turista, cant_Filas_Turista, modelo, aircraftSize, maxWeight)
VALUES 
(1, 4, 3, 6, 27, 'PI-Sprint2', 'Mediana', 70000),
(1, 2, 2, 4, 20, 'AIRBUS-2026', 'Mediana', 75000);

INSERT INTO Airport (codeAirport, adminID, nameAirport, city, country, timeZone)
VALUES 
('SJO', 1, 'Juan Santamaría International', 'San José', 'Costa Rica', 'UTC-6'),
('CDG', 1, 'Charles de Gaulle', 'Paris', 'France', 'UTC+1'),
('JFK', 1, 'John F. Kennedy International', 'New York', 'USA', 'UTC-5'),
('MEL', 1, 'Melbourne Airport', 'Melbourne', 'Australia', 'UTC+10'),
('MSQ', 1, 'Minsk National Airport', 'Minsk', 'Belarus', 'UTC+3'),
('MAD', 1, 'Adolfo Suárez Madrid-Barajas', 'Madrid', 'Spain', 'UTC+1'),
('FRA', 1, 'Frankfurt Airport', 'Frankfurt', 'Germany', 'UTC+1'),
('AMS', 1, 'Amsterdam Airport Schiphol', 'Amsterdam', 'Netherlands', 'UTC+1');

INSERT INTO Route (adminID, codeAirportSalida, codeAirportLlegada, modelo, firstClassPrice, turistClassPrice, stimatedTime, distance, luggagePrice, carryOnPrice, carryOnMaxWeight, luggageMaxWeight, porcentageMultiplier)
VALUES 
(1, 'SJO', 'CDG', 'PI-Sprint2', 2000, 800,'09:00', 9000, 100, 0, 2300, 1000, 0.50),
(1, 'SJO', 'JFK', 'PI-Sprint2', 1000, 400,'05:00', 3500, 75, 0, 2300, 1000, 0.50),
(1, 'JFK', 'CDG', 'PI-Sprint2', 1050, 450,'07:00', 5800, 75, 0, 2300, 1000, 0.50),
(1, 'JFK', 'CDG', 'PI-Sprint2', 800, 350,'07:00', 5800, 75, 0, 2300, 1000, 0.50),
(1, 'CDG', 'MEL', 'PI-Sprint2', 1000, 300,'03:00', 2000, 50, 0, 2300, 1000, 0.50),
(1, 'MSQ', 'SJO', 'AIRBUS-2026', 2200, 950,'10:00', 10000, 100, 0, 2300, 1000,  0.50),
(1, 'SJO', 'MAD', 'PI-Sprint2', 1999, 799,'09:00', 8500, 100, 0, 2300, 1000, 0.50),
(1, 'SJO', 'FRA', 'PI-Sprint2', 1950, 790,'09:00', 9200, 100, 0, 2300, 1000, 0.50),
(1, 'SJO', 'AMS', 'PI-Sprint2', 2050, 810,'09:00', 8800, 100, 0, 2300, 1000, 0.50);
Select * from Route
INSERT INTO FlightFrequency (idRoute, dayOfWeek, departureTime, estimatedArrivalTime, startingDate, endingDate, active)
VALUES 
-- #1: SJO-CDG (L y J)
(1, 'Monday', '07:00:00', '23:00:00', '2026-06-01', '2026-12-31', 1),
(1, 'Thursday', '07:00:00', '23:00:00', '2026-06-01', '2026-12-31', 1),
-- #2: SJO-JFK (D y J)
(2, 'Sunday', '08:00:00', '13:00:00', '2026-06-01', '2026-12-31', 1),
(2, 'Thursday', '08:00:00', '13:00:00', '2026-06-01', '2026-12-31', 1),
-- #3: JFK-CDG (L, J y S)
(3, 'Monday', '11:00:00', '18:00:00', '2026-06-01', '2026-12-31', 1),
(3, 'Thursday', '11:00:00', '18:00:00', '2026-06-01', '2026-12-31', 1),
(3, 'Saturday', '11:00:00', '18:00:00', '2026-06-01', '2026-12-31', 1),
-- #4: JFK-CDG (L, J y S)
(4, 'Monday', '17:00:00', '00:00:00', '2026-06-01', '2026-12-31', 1),
(4, 'Thursday', '17:00:00', '00:00:00', '2026-06-01', '2026-12-31', 1),
(4, 'Saturday', '17:00:00', '00:00:00', '2026-06-01', '2026-12-31', 1),
-- #5: CDG-MEL (V)
(5, 'Friday', '05:00:00', '08:00:00', '2026-06-01', '2026-12-31', 1),
-- #6: MSQ-SJO (J)
(6, 'Thursday', '01:00:00', '04:00:00', '2026-06-01', '2026-12-31', 1),
-- #7: SJO-MAD (L y J)
(7, 'Monday', '06:30:00', '15:30:00', '2026-06-01', '2026-12-31', 1),
(7, 'Thursday', '06:30:00', '15:30:00', '2026-06-01', '2026-12-31', 1),
-- #8: SJO-FRA (L y J)
(8, 'Monday', '06:45:00', '15:45:00', '2026-06-01', '2026-12-31', 1),
(8, 'Thursday', '06:45:00', '15:45:00', '2026-06-01', '2026-12-31', 1),
-- #9: SJO-AMS (L y J)
(9, 'Monday', '07:15:00', '16:15:00', '2026-06-01', '2026-12-31', 1),
(9, 'Thursday', '07:15:00', '16:15:00', '2026-06-01', '2026-12-31', 1);
GO



Select * from Aircraft;
Select * from Luggage;
Select * from Passenger;
Select * from FlightFrequency;
Select * from Flight;
Select * from Route;
Select * from Itinerary;
Select * from Luggage;
Select * from PassengerItinerary;
Select * from Registra;

Select * from Tiene;

