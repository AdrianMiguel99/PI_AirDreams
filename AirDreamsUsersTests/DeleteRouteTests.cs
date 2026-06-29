using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Moq;

public class DeleteRouteTests
{
    private Mock<IRouteRepository> _routeRepository;
    private RouteService _service;

    [SetUp]
    public void Setup()
    {
        _routeRepository = new Mock<IRouteRepository>();
        _service = new RouteService(_routeRepository.Object);
    }

    [Test]
    public async Task DeleteRoute_shouldReturnSoftDeleteResult_whenRepositoryReturnsSoftDelete()
    {
        // Arrange
        var expectedResult = new RouteDeleteResultDTO
        {
            Message = "La ruta posee vuelos asociados y fue desactivada."
        };

        _routeRepository.Setup(x => x.DeleteAsync(1)).ReturnsAsync(expectedResult);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.That(result.Message, Is.EqualTo("La ruta posee vuelos asociados y fue desactivada."));
        _routeRepository.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task DeleteRoute_shouldReturnHardDeleteResult_whenRepositoryReturnsHardDelete()
    {
        // Arrange
        var expectedResult = new RouteDeleteResultDTO
        {
            Message = "La ruta fue eliminada permanentemente."
        };

        _routeRepository.Setup(x => x.DeleteAsync(2)).ReturnsAsync(expectedResult);

        // Act
        var result = await _service.DeleteAsync(2);

        // Assert
        Assert.That(result.Message, Is.EqualTo("La ruta fue eliminada permanentemente."));
        _routeRepository.Verify(x => x.DeleteAsync(2), Times.Once);
    }

    [Test]
    public void DeleteRoute_shouldThrowArgumentException_whenRouteIdIsZero()
    {
        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(0));

        Assert.That(exception!.Message, Is.EqualTo("El ID de la ruta no es válido."));
        _routeRepository.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void DeleteRoute_shouldThrowArgumentException_whenRouteIdIsNegative()
    {
        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteAsync(-1));

        Assert.That(exception!.Message, Is.EqualTo("El ID de la ruta no es válido."));
        _routeRepository.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
