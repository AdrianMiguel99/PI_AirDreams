using AirDreams.API.DTOs;
using AirDreams.API.Services;
using AirDreams.ExternalAPI.DTOs;

public class FlightConnectorServiceTests
{
    private readonly FlightConnectorService _service;

    public FlightConnectorServiceTests()
    {
        _service = new FlightConnectorService();
    }

    [Test]
    public void ConnectFlights_shouldReturnConnectedFlight_whenAirportsMatchAndLayoverWithinRange()
    {
        // Arrange
        var internalFlight = BuildInternalFlight("JFK", "08:00", 200, 500);
        var externalFlight = BuildExternalFlight("JFK", "2026-06-21T10:00:00", 40, 90);

        // Act
        var result = _service.ConnectFlights(
            new List<FlightSegmentDTO> { internalFlight },
            new List<ExternalResponseFlightDTO> { externalFlight }
        );

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].LayoverHours, Is.EqualTo(2));
        Assert.That(result[0].TouristPrice, Is.EqualTo(240));
        Assert.That(result[0].FirstClassPrice, Is.EqualTo(590));
    }

    [Test]
    public void ConnectFlights_shouldReturnEmptyList_whenArrivalAirportDoesNotMatchDepartureAirport()
    {
        // Arrange
        var internalFlight = BuildInternalFlight("JFK", "08:00", 200, 500);
        var externalFlight = BuildExternalFlight("MIA", "2026-06-21T10:00:00", 40, 90);

        // Act
        var result = _service.ConnectFlights(
            new List<FlightSegmentDTO> { internalFlight },
            new List<ExternalResponseFlightDTO> { externalFlight }
        );

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ConnectFlights_shouldReturnEmptyList_whenLayoverIsBelowMinimum()
    {
        // Arrange
        var internalFlight = BuildInternalFlight("JFK", "08:00", 200, 500);
        var externalFlight = BuildExternalFlight("JFK", "2026-06-21T09:00:00", 40, 90);

        // Act
        var result = _service.ConnectFlights(
            new List<FlightSegmentDTO> { internalFlight },
            new List<ExternalResponseFlightDTO> { externalFlight }
        );

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ConnectFlights_shouldReturnEmptyList_whenLayoverIsAboveMaximum()
    {
        // Arrange
        var internalFlight = BuildInternalFlight("JFK", "08:00", 200, 500);
        var externalFlight = BuildExternalFlight("JFK", "2026-06-21T21:00:00", 40, 90);

        // Act
        var result = _service.ConnectFlights(
            new List<FlightSegmentDTO> { internalFlight },
            new List<ExternalResponseFlightDTO> { externalFlight }
        );

        // Assert
        Assert.That(result, Is.Empty);
    }

    private FlightSegmentDTO BuildInternalFlight(
        string arrivalAirportCode,
        string arrivalTime,
        decimal touristPrice,
        decimal firstClassPrice)
    {
        return new FlightSegmentDTO
        {
            ArrivalDate = new DateTime(2026, 6, 21),
            ArrivalTime = arrivalTime,
            ArrivalAirport = new AirportDTO { Code = arrivalAirportCode },
            TouristPrice = touristPrice,
            FirstClassPrice = firstClassPrice
        };
    }

    private ExternalResponseFlightDTO BuildExternalFlight(
        string departureAirportCode,
        string departureTime,
        decimal touristPrice,
        decimal firstClassPrice)
    {
        return new ExternalResponseFlightDTO
        {
            departureAirport = new ExternalResponseAirportDTO { code = departureAirportCode },
            departureTime = departureTime,
            touristPrice = touristPrice,
            firstClassPrice = firstClassPrice
        };
    }
}