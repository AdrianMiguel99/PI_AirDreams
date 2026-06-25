using AirDreams.API.Repositories;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using AirDreams.API.Models;
using AirDreams.API.DTOs;
using Moq;

public class CheckAvailabilityTests
{
    private readonly Mock<ILuggageRepository> _luggageRepository;
    private readonly Mock<IPassengerService> _passengerService;
    private readonly LuggageService _service;

    public CheckAvailabilityTests()
    {
        _luggageRepository = new Mock<ILuggageRepository>();
        _passengerService = new Mock<IPassengerService>();
        _service = new LuggageService(_luggageRepository.Object, _passengerService.Object);
    }

    [Test]
    public async Task CheckAvailability_shouldReturnSuccess_whenEnoughSpaceExists()
    {
        // Arrange
        _luggageRepository.Setup(x => x.CheckLuggageWeightAsync("AD101", 1, 50)).ReturnsAsync(true);
        _luggageRepository.Setup(x => x.CheckCarryOnWeightAsync("AD101", 1, 20)).ReturnsAsync(true);

        // Act
        var result = await _service.CheckAvailabilityAsync("AD101", 1, 50, 20);

        // Assert
        Assert.That(result.success, Is.True);
        Assert.That(result.message, Is.EqualTo("Hay espacio suficiente para el equipaje y el equipaje de mano"));
    }

    [Test]
    public async Task CheckAvailability_shouldReturnFailure_whenNotEnoughSpaceForLuggageExists()
    {
        // Arrange
        _luggageRepository.Setup(x => x.CheckLuggageWeightAsync("AD101", 1, 50)).ReturnsAsync(false);
        _luggageRepository.Setup(x => x.CheckCarryOnWeightAsync("AD101", 1, 20)).ReturnsAsync(true);

        // Act
        var result = await _service.CheckAvailabilityAsync("AD101", 1, 50, 20);

        // Assert
        Assert.That(result.success, Is.False);
        Assert.That(result.message, Is.EqualTo("No hay espacio suficiente para el equipaje"));
    }

    [Test]
    public async Task CheckAvailability_shouldReturnFailure_whenNotEnoughSpaceForCarryOnExists()
    {
        // Arrange
        _luggageRepository.Setup(x => x.CheckLuggageWeightAsync("AD101", 1, 50)).ReturnsAsync(true);
        _luggageRepository.Setup(x => x.CheckCarryOnWeightAsync("AD101", 1, 20)).ReturnsAsync(false);

        // Act
        var result = await _service.CheckAvailabilityAsync("AD101", 1, 50, 20);

        // Assert
        Assert.That(result.success, Is.False);
        Assert.That(result.message, Is.EqualTo("No hay espacio suficiente para el equipaje de mano"));
    }

    [Test]
    public async Task CheckAvailability_shouldReturnFailure_whenNotEnoughSpaceForBothExists()
    {
        // Arrange
        _luggageRepository.Setup(x => x.CheckLuggageWeightAsync("AD101", 1, 50)).ReturnsAsync(false);
        _luggageRepository.Setup(x => x.CheckCarryOnWeightAsync("AD101", 1, 20)).ReturnsAsync(false);

        // Act
        var result = await _service.CheckAvailabilityAsync("AD101", 1, 50, 20);

        // Assert
        Assert.That(result.success, Is.False);
        Assert.That(result.message, Is.EqualTo("No hay espacio suficiente para el equipaje y el equipaje de mano"));
    }
}