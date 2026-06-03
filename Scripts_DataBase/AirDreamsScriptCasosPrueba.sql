USE AirDreams;
GO
DELETE FROM Tiene
DELETE FROM Flight;
DELETE FROM FlightFrequency;

DELETE FROM Route;

DELETE FROM Aircraft;
DELETE FROM Airport;


Select * FROM Route;

GO

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

INSERT INTO Route (adminID, codeAirportSalida, codeAirportLlegada, modelo, firstClassPrice, turistClassPrice, stimatedTime, distance, luggagePrice, carryOnPrice, porcentageMultiplier)
VALUES 
(1, 'SJO', 'CDG', 'PI-Sprint2', 2000, 800,'09:00', 9000, 100, 0, 0.50),
(1, 'SJO', 'JFK', 'PI-Sprint2', 1000, 400,'05:00', 3500, 75, 0, 0.50),
(1, 'JFK', 'CDG', 'PI-Sprint2', 1050, 450,'07:00', 5800, 75, 0, 0.50),
(1, 'JFK', 'CDG', 'PI-Sprint2', 800, 350,'07:00', 5800, 75, 0, 0.50),
(1, 'CDG', 'MEL', 'PI-Sprint2', 1000, 300,'03:00', 2000, 50, 0, 0.50),
(1, 'MSQ', 'SJO', 'AIRBUS-2026', 2200, 950,'10:00', 10000, 100, 0, 0.50),
(1, 'SJO', 'MAD', 'PI-Sprint2', 1999, 799,'09:00', 8500, 100, 0, 0.50),
(1, 'SJO', 'FRA', 'PI-Sprint2', 1950, 790,'09:00', 9200, 100, 0, 0.50),
(1, 'SJO', 'AMS', 'PI-Sprint2', 2050, 810,'09:00', 8800, 100, 0, 0.50);
Select * from Route
INSERT INTO FlightFrequency (idRoute, dayOfWeek, departureTime, estimatedArrivalTime, startingDate, endingDate, active)
VALUES 
-- #1: SJO-CDG (L y J)
(1023, 'Monday', '07:00:00', '23:00:00', '2026-06-01', '2026-12-31', 1),
(1023, 'Thursday', '07:00:00', '23:00:00', '2026-06-01', '2026-12-31', 1),
-- #2: SJO-JFK (D y J)
(1024, 'Sunday', '08:00:00', '13:00:00', '2026-06-01', '2026-12-31', 1),
(1024, 'Thursday', '08:00:00', '13:00:00', '2026-06-01', '2026-12-31', 1),
-- #3: JFK-CDG (L, J y S)
(1025, 'Monday', '11:00:00', '18:00:00', '2026-06-01', '2026-12-31', 1),
(1025, 'Thursday', '11:00:00', '18:00:00', '2026-06-01', '2026-12-31', 1),
(1025, 'Saturday', '11:00:00', '18:00:00', '2026-06-01', '2026-12-31', 1),
-- #4: JFK-CDG (L, J y S)
(1026, 'Monday', '17:00:00', '00:00:00', '2026-06-01', '2026-12-31', 1),
(1026, 'Thursday', '17:00:00', '00:00:00', '2026-06-01', '2026-12-31', 1),
(1026, 'Saturday', '17:00:00', '00:00:00', '2026-06-01', '2026-12-31', 1),
-- #5: CDG-MEL (V)
(1027, 'Friday', '05:00:00', '08:00:00', '2026-06-01', '2026-12-31', 1),
-- #6: MSQ-SJO (J)
(1028, 'Thursday', '01:00:00', '04:00:00', '2026-06-01', '2026-12-31', 1),
-- #7: SJO-MAD (L y J)
(1029, 'Monday', '06:30:00', '15:30:00', '2026-06-01', '2026-12-31', 1),
(1029, 'Thursday', '06:30:00', '15:30:00', '2026-06-01', '2026-12-31', 1),
-- #8: SJO-FRA (L y J)
(1030, 'Monday', '06:45:00', '15:45:00', '2026-06-01', '2026-12-31', 1),
(1030, 'Thursday', '06:45:00', '15:45:00', '2026-06-01', '2026-12-31', 1),
-- #9: SJO-AMS (L y J)
(1031, 'Monday', '07:15:00', '16:15:00', '2026-06-01', '2026-12-31', 1),
(1031, 'Thursday', '07:15:00', '16:15:00', '2026-06-01', '2026-12-31', 1);
GO