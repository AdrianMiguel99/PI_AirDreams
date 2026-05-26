-- Agregamos la tabla de tipoAeronave
ALTER TABLE Aircraft
ADD aircraftSize VARCHAR(20);
GO

-- Seteamos como "No definido" los campos dado que las aeronaves ya creadas tendrian en null
-- La idea es que se pueda agregar el tamano en la opcion de editar aeronave.
UPDATE Aircraft
SET aircraftSize = 'No definido'
WHERE aircraftSize IS NULL;
GO

-- Agregamos las restricciones del aircraftSize
ALTER TABLE Aircraft
ALTER COLUMN aircraftSize VARCHAR(20) NOT NULL;
GO

ALTER TABLE Aircraft
ADD CONSTRAINT CHK_Aircraft_Tamano
CHECK (aircraftSize IN ('Pequena', 'Mediana','Grande', 'No definido'));
GO

-- Hacemos la migracion de Route a que apunte a la nueva PK modelo de aeronave.
-- Quitamos la restriccion de la fk de route.
ALTER TABLE Route
DROP CONSTRAINT fk_route_aircraft;
GO

--agregamos modelo como columna de route
ALTER TABLE Route
ADD modelo VARCHAR(100) NULL;
GO

--seteamos el modelo correspondiente segun la matricula de la aeronave
UPDATE R
SET R.modelo = A.modelo
FROM Route R
INNER JOIN Aircraft A
    ON R.plateNumber = A.plateNumber;
GO

--eliminamos la columna de plateNumber de route.
ALTER TABLE Route
DROP COLUMN plateNumber;
GO

--Quitamos la pk de Aircraft y borramos el plateNumber y la columnna del total de pasajeros
ALTER TABLE Aircraft
DROP CONSTRAINT PK__Aircraft__04A7B9BE5D240E72;
GO

ALTER TABLE Aircraft
DROP COLUMN plateNumber;
GO

--eliminamos check antiguo
ALTER TABLE Aircraft
DROP CONSTRAINT CK__Aircraft__cantPa__5629CD9C;
GO

ALTER TABLE Aircraft
DROP COLUMN cantPasajeros;
GO

--Terminamos de construir el modelo en aircraft
--seteamos como not null
ALTER TABLE Aircraft
ALTER COLUMN modelo VARCHAR(100) NOT NULL;
GO

-- creamos la nueva restriccion de PK
ALTER TABLE Aircraft
ADD CONSTRAINT PK_Aircraft_Modelo
PRIMARY KEY (modelo);
GO

--seteamos el FK de route hacia aircraft
ALTER TABLE Route
ALTER COLUMN modelo VARCHAR(100) NOT NULL;
GO

ALTER TABLE Route
ADD CONSTRAINT FK_Route_Aircraft_Modelo
FOREIGN KEY (modelo)
REFERENCES Aircraft(modelo);
GO

