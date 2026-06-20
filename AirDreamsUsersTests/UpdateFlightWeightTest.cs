using AirDreams.API.Repositories;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using AirDreams.API.Models;
using AirDreams.API.DTOs;
using Moq;

public class UpdateFlightWeightTests
{
    private readonly Mock<ILuggageRepository> _luggageRepository;
    private readonly Mock<IPassengerService> _passengerService;
    private readonly LuggageService _service;

    public UpdateFlightWeightTests()
    {
        _luggageRepository = new Mock<ILuggageRepository>();
        _passengerService = new Mock<IPassengerService>();
        _service = new LuggageService(_luggageRepository.Object, _passengerService.Object);
    }

    [Test]
    public async Task UpdateFlightWeight_shouldReturnSuccess_whenWeightUpdated()
    {
        // Arrange
        _luggageRepository.Setup(x => x.UpdateFlightWeightAsync("FL123", 100, 30)).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateFlightWeightAsync("FL123", 100, 30);

        // Assert
        Assert.That(result.success, Is.True);
        Assert.That(result.message, Is.EqualTo("Peso actualizado correctamente"));
    }

    [Test]
    public async Task UpdateFlightWeight_shouldReturnFailure_whenWeightNotUpdated()
    {
        // Arrange
        _luggageRepository.Setup(x => x.UpdateFlightWeightAsync("FL123", 100, 30)).ReturnsAsync(false);

        // Act
        var result = await _service.UpdateFlightWeightAsync("FL123", 100, 30);

        // Assert
        Assert.That(result.success, Is.False);
        Assert.That(result.message, Is.EqualTo("No fue posible actualizar el peso del vuelo"));
    }
}