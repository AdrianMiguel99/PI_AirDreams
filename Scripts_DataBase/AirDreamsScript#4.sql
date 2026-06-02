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
CHECK (aircraftSize IN ('Pequena', 'Mediana', 'Grande', 'No definido'));
GO

-- Hacemos la migracion de Route a que apunte a la nueva PK modelo de aeronave.
-- Quitamos la restriccion de la fk de route.
DECLARE @fkName NVARCHAR(200);
DECLARE @sql NVARCHAR(MAX);

SELECT @fkName = fk.name
FROM sys.foreign_keys fk
WHERE fk.parent_object_id = OBJECT_ID('Route')
  AND fk.referenced_object_id = OBJECT_ID('Aircraft');

IF @fkName IS NOT NULL
BEGIN
    SET @sql = 'ALTER TABLE Route DROP CONSTRAINT ' + QUOTENAME(@fkName);
    EXEC sp_executesql @sql;
END
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
DECLARE @pkName NVARCHAR(200);
DECLARE @sql NVARCHAR(MAX);

SELECT @pkName = kc.name
FROM sys.key_constraints kc
WHERE kc.parent_object_id = OBJECT_ID('Aircraft')
  AND kc.type = 'PK';

IF @pkName IS NOT NULL
BEGIN
    SET @sql = 'ALTER TABLE Aircraft DROP CONSTRAINT ' + QUOTENAME(@pkName);
    EXEC sp_executesql @sql;
END
GO

ALTER TABLE Aircraft
DROP COLUMN plateNumber;
GO

--eliminamos check antiguo
DECLARE @checkName NVARCHAR(200);
DECLARE @sql NVARCHAR(MAX);

SELECT @checkName = cc.name
FROM sys.check_constraints cc
WHERE cc.parent_object_id = OBJECT_ID('Aircraft')
  AND cc.definition LIKE '%cantPasajeros%';

IF @checkName IS NOT NULL
BEGIN
    SET @sql = 'ALTER TABLE Aircraft DROP CONSTRAINT ' + QUOTENAME(@checkName);
    EXEC sp_executesql @sql;
END
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