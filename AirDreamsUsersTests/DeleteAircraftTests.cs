using AirDreams.API.DTOs;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Moq;
using NUnit.Framework;

namespace Unitest_Aircraft
{
    public class DeleteAircraftTests
    {
        private Mock<IAircraftRepository> _aircraftRepository;
        private AircraftService _service;

        [SetUp]
        public void Setup()
        {
            _aircraftRepository = new Mock<IAircraftRepository>();
            _service = new AircraftService(_aircraftRepository.Object);
        }

        [Test]
        public void DeleteAircraft_shouldReturnMessage_whenRepositoryDeletesAircraft()
        {
            // Arrange
            var expectedResult = new AircraftDeleteResultDTO
            {
                Message = "La aeronave fue eliminada permanentemente."
            };

            _aircraftRepository
                .Setup(x => x.DeleteAircraft("BOEING-TEST"))
                .Returns(expectedResult);

            // Act
            var result = _service.DeleteAircraft("BOEING-TEST");

            // Assert
            Assert.That(result.Message, Is.EqualTo("La aeronave fue eliminada permanentemente."));
            _aircraftRepository.Verify(x => x.DeleteAircraft("BOEING-TEST"), Times.Once);
        }

        [Test]
        public void DeleteAircraft_shouldReturnMessage_whenRepositorySoftDeletesAircraft()
        {
            // Arrange
            var expectedResult = new AircraftDeleteResultDTO
            {
                Message = "La aeronave posee rutas con vuelos asociados y fue desactivada."
            };

            _aircraftRepository
                .Setup(x => x.DeleteAircraft("AIRBUS-TEST"))
                .Returns(expectedResult);

            // Act
            var result = _service.DeleteAircraft("AIRBUS-TEST");

            // Assert
            Assert.That(result.Message, Is.EqualTo("La aeronave posee rutas con vuelos asociados y fue desactivada."));
            _aircraftRepository.Verify(x => x.DeleteAircraft("AIRBUS-TEST"), Times.Once);
        }

        [Test]
        public void DeleteAircraft_shouldThrowArgumentException_whenModelIsEmpty()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _service.DeleteAircraft(""));

            Assert.That(exception!.Message, Is.EqualTo("El modelo es obligatorio."));
            _aircraftRepository.Verify(x => x.DeleteAircraft(It.IsAny<string>()), Times.Never);
        }
    }
}
