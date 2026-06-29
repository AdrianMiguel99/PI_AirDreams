CREATE OR ALTER PROCEDURE dbo.GetReservationDetails
    @ReservationCode VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- pasajeros de la reserva
    SELECT
        pi.transactionId,
        p.idPassenger,
        CONCAT(p.namePassenger, ' ', p.lastnamesPassenger) AS passengerName
    FROM dbo.PassengerItinerary pi
    INNER JOIN dbo.Passenger p
        ON p.idPassenger = pi.idPassenger
    WHERE pi.transactionId = @ReservationCode;


    -- vuelos de la reserva
    SELECT
        t.transactionId,
        t.flightNumber,
        f.departureDate,
        f.flightState,

        r.codeAirportSalida AS originCode,
        originAirport.city AS originCity,
        originAirport.country AS originCountry,

        r.codeAirportLlegada AS destinationCode,
        destinationAirport.city AS destinationCity,
        destinationAirport.country AS destinationCountry,

        r.modelo AS aircraftModel,
        r.stimatedTime

    FROM dbo.Tiene t
    INNER JOIN dbo.Flight f
        ON f.numberFlight = t.flightNumber
    INNER JOIN dbo.Route r
        ON r.idRoute = f.routeId
    INNER JOIN dbo.Airport originAirport
        ON originAirport.codeAirport = r.codeAirportSalida
    INNER JOIN dbo.Airport destinationAirport
        ON destinationAirport.codeAirport = r.codeAirportLlegada
    WHERE t.transactionId = @ReservationCode;

END;

