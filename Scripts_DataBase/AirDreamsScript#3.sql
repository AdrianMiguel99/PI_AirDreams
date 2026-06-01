Use AirDreams
-- Se agregan los campos para el metodo de pago al itinerario
ALTER TABLE Itinerary
ADD paymentMethod VARCHAR(20) NULL,
    cardLastFour VARCHAR(4) NULL,
    buyerName VARCHAR(100) NULL,
    paymentDate DATETIME NULL;

-- Aumenté el tamaño de flightNumber por que me estaba dando errores a la hora de crear el vuelo correr paso a paso

-- Paso 1: Eliminar restricciones 
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'fk_tiene_flight')
    ALTER TABLE Tiene DROP CONSTRAINT fk_tiene_flight;
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'fk_checkin_flight')
    ALTER TABLE CheckIn DROP CONSTRAINT fk_checkin_flight;

-- Paso 2: Alterar todas las columnas involucradas a VARCHAR(10) para que coincidan
ALTER TABLE Flight ALTER COLUMN numberFlight VARCHAR(10) NOT NULL;
ALTER TABLE Tiene ALTER COLUMN flightNumber VARCHAR(10) NOT NULL;
ALTER TABLE CheckIn ALTER COLUMN flightNumber VARCHAR(10) NOT NULL;

-- Paso 3: Recrear las restricciones de clave foránea
ALTER TABLE Tiene ADD CONSTRAINT fk_tiene_flight FOREIGN KEY (flightNumber) REFERENCES Flight(numberFlight);
ALTER TABLE CheckIn ADD CONSTRAINT fk_checkin_flight FOREIGN KEY (flightNumber) REFERENCES Flight(numberFlight);
