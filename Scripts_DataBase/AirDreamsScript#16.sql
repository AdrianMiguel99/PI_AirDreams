Use AirDreams
GO

CREATE OR ALTER PROCEDURE dbo.GetReservationDetails
    @ReservationCode VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    -- pasajeros
    SELECT
        pi.transactionId,
        p.idPassenger,
        CONCAT(p.namePassenger, ' ', p.lastnamesPassenger) AS passengerName,
        pi.seatNumber
    FROM dbo.PassengerItinerary pi
    INNER JOIN dbo.Passenger p
        ON p.idPassenger = pi.idPassenger
    WHERE pi.transactionId = @ReservationCode;


    -- vuelos propios
    SELECT
        t.transactionId,
        i.itineraryStatus AS itineraryStatus,
        i.seatClass AS seatClass,

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
        r.stimatedTime AS duration,

        'AirDreams' AS airlineName,
        1 AS isAirDreams

    FROM dbo.Tiene t
    INNER JOIN dbo.Itinerary i
        ON i.transactionId = t.transactionId
    INNER JOIN dbo.Flight f
        ON f.numberFlight = t.flightNumber
    INNER JOIN dbo.Route r
        ON r.idRoute = f.routeId
    INNER JOIN dbo.Airport originAirport
        ON originAirport.codeAirport = r.codeAirportSalida
    INNER JOIN dbo.Airport destinationAirport
        ON destinationAirport.codeAirport = r.codeAirportLlegada
    WHERE t.transactionId = @ReservationCode

    UNION ALL

    -- vuelos externos
    SELECT
        te.transactionId,
        i.itineraryStatus AS itineraryStatus,
        i.seatClass AS seatClass,

        te.externalFlightNumber AS flightNumber,
        ef.departureDateTime AS departureDate,
        'External' AS flightState,

        ef.departureAirportCode AS originCode,
        originAirport.city AS originCity,
        originAirport.country AS originCountry,

        ef.arrivalAirportCode AS destinationCode,
        destinationAirport.city AS destinationCity,
        destinationAirport.country AS destinationCountry,

        NULL AS aircraftModel,
        ef.duration AS duration,

        ef.partnerName AS airlineName,
        0 AS isAirDreams

    FROM dbo.TieneExternal te
    INNER JOIN dbo.Itinerary i
        ON i.transactionId = te.transactionId
    INNER JOIN dbo.ExternalFlight ef
        ON ef.flightNumber = te.externalFlightNumber
    INNER JOIN dbo.Airport originAirport
        ON originAirport.codeAirport = ef.departureAirportCode
    INNER JOIN dbo.Airport destinationAirport
        ON destinationAirport.codeAirport = ef.arrivalAirportCode
    WHERE te.transactionId = @ReservationCode;

END;