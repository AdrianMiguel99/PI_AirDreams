using AirDreams.API.Repositories;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using AirDreams.API.Models;
using AirDreams.API.DTOs;
using Moq;

public class RegisterLuggageTests
{
    private readonly Mock<ILuggageRepository> _luggageRepository;
    private readonly Mock<IPassengerService> _passengerService;
    private readonly LuggageService _service;

    public RegisterLuggageTests()
    {
        _luggageRepository = new Mock<ILuggageRepository>();
        _passengerService = new Mock<IPassengerService>();
        _service = new LuggageService(_luggageRepository.Object, _passengerService.Object);
    }

    [Test]
    public async Task RegisterLuggage_shouldReturnSuccess_whenLuggageRegistered()
    {
        // arrange
        var passenger = new PassengerDTO
        {
            IdPassenger = 1
        };
        var model = new LuggageRegistrationModel
        {
            FullName = "Dylan Torres",
            TransactionIdItinerary = "TX123",
            LuggageItems =
            [
                new LuggageItemModel
                {
                    Type = "checked",
                    Quantity = 2
                }
            ]
        };
        _passengerService.Setup(x => x.GetPassengerByNameAsync(model.FullName)).ReturnsAsync(passenger);
        _luggageRepository.Setup(x => x.CreateLuggageAsync("checked", 2)).ReturnsAsync("LUG001");
        _luggageRepository.Setup(x => x.RegisterLuggageAsync(passenger.IdPassenger, model.TransactionIdItinerary, "LUG001")).ReturnsAsync(true);

        // act
        var result = await _service.RegisterLuggageAsync(model);

        // assert
        Assert.That(result.success, Is.True);
    }

    [Test]
    public async Task RegisterLuggage_shouldReturnFailure_whenPassengerNotFound()
    {
        // arrange
        var model = new LuggageRegistrationModel
        {
            FullName = "Dylan Torres",
            TransactionIdItinerary = "TX123",
            LuggageItems =
            [
                new LuggageItemModel
                {
                    Type = "checked",
                    Quantity = 2
                }
            ]
        };
        _passengerService.Setup(x => x.GetPassengerByNameAsync(model.FullName)).ReturnsAsync((PassengerDTO)null!);

        // act
        var result = await _service.RegisterLuggageAsync(model);

        // assert
        Assert.That(result.success, Is.False);
    }

    [Test]
    public async Task RegisterLuggage_shouldReturnFailure_whenNoLuggageRegistered()
    {
        // arrange
        var passenger = new PassengerDTO
        {
            IdPassenger = 1
        };
        var model = new LuggageRegistrationModel
        {
            FullName = "Dylan Torres",
            TransactionIdItinerary = "TX123",
            LuggageItems =
            [
                new LuggageItemModel
                {
                    Type = "checked",
                    Quantity = 2
                }
            ]
        };
        _passengerService.Setup(x => x.GetPassengerByNameAsync(model.FullName)).ReturnsAsync(passenger);
        _luggageRepository.Setup(x => x.CreateLuggageAsync("checked", 2)).ReturnsAsync("LUG001");
        _luggageRepository.Setup(x => x.RegisterLuggageAsync(passenger.IdPassenger, model.TransactionIdItinerary, "LUG001")).ReturnsAsync(false);

        // act
        var result = await _service.RegisterLuggageAsync(model);

        // assert
        Assert.That(result.success, Is.False);
    }


}