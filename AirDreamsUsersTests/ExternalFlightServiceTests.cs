using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services;
using AirDreams.ExternalAPI.DTOs;
using Moq;

public class ExternalFlightServiceTests
{
    private Mock<IExternalFlightRepository> _repositoryMock;
    private ExternalFlightService _service;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IExternalFlightRepository>();
        _service = new ExternalFlightService(_repositoryMock.Object);
    }

    [Test]
    public async Task RegisterExternalFlightAsync_shouldCallRepositoryWithMushuAirlines_whenFlightStartsWithMU()
    {
        // Arrange
        var flight = new ExternalResponseFlightDTO { flightGUID = "MU0007" };

        // Act
        await _service.RegisterExternalFlightAsync("TX001", flight);

        // Assert
        _repositoryMock.Verify(r => r.RegisterExternalFlightAsync("TX001", flight, "Mushu Airlines"), Times.Once);
    }

    [Test]
    public async Task RegisterExternalFlightAsync_shouldCallRepositoryWithSnoopyAirlines_whenFlightStartsWithSN()
    {
        // Arrange
        var flight = new ExternalResponseFlightDTO { flightGUID = "SN1234" };

        // Act
        await _service.RegisterExternalFlightAsync("TX001", flight);

        // Assert
        _repositoryMock.Verify(r => r.RegisterExternalFlightAsync("TX001", flight, "Snoopy Airlines"), Times.Once);
    }

    [Test]
    public async Task RegisterExternalFlightAsync_shouldCallRepositoryWithZuliAirlines_whenFlightStartsWithZU()
    {
        // Arrange
        var flight = new ExternalResponseFlightDTO { flightGUID = "ZU9999" };

        // Act
        await _service.RegisterExternalFlightAsync("TX001", flight);

        // Assert
        _repositoryMock.Verify(r => r.RegisterExternalFlightAsync("TX001", flight, "Zuli Airlines"), Times.Once);
    }

}
