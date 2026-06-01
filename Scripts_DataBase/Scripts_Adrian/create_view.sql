-- creamos una funcion que recive como parametro la cantidad asientos y asientos por fila de ambas clases para generar un total de pasajeros del avion
CREATE FUNCTION CalcularPasajerosAeronave
(
    @asientosFilaFirst INT,
    @filasFirst INT,
    @asientosFilaTurista INT,
    @filasTurista INT
)
RETURNS INT
AS
BEGIN
    RETURN
        (@asientosFilaFirst * @filasFirst)
        +
        (@asientosFilaTurista * @filasTurista)
END;
GO


--La funcion sera utilizada en un view que mostrara los datos de la aeronave y como extra el total de pasajeros.
CREATE OR ALTER VIEW AircraftView
AS

SELECT
    modelo,
    aircraftSize,
    maxWeight,

    cant_Asientos_Fila_Firstclass,
    cant_Filas_Firstclass,

    cant_Asientos_Fila_Turista,
    cant_Filas_Turista,

    dbo.CalcularPasajerosAeronave(
        cant_Asientos_Fila_Firstclass,
        cant_Filas_Firstclass,
        cant_Asientos_Fila_Turista,
        cant_Filas_Turista
    ) AS cantPasajeros

FROM Aircraft;
GO

SELECT *
FROM AircraftView;
GO