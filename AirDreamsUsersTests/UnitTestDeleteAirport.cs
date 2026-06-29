using Moq;
using NUnit.Framework;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace Test1
{
    public class UnitTestDeleteAirport
    {
        private Mock<IAirportRepository> _repositoryMock;
        private AirportService _service;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IAirportRepository>();
            _service = new AirportService(_repositoryMock.Object);
        }

        [Test]
        public void DeleteAsync_NonExistingAirport_ReturnsError()
        {
            var code = "XYZ";
            _repositoryMock.Setup(r => r.GetByCodeAsync(code))
                           .ReturnsAsync((Airport?)null);

            var result = _service.DeleteAsync(code).Result;

            Assert.That(result.success, Is.False);
            Assert.That(result.message, Is.EqualTo("Aeropuerto no encontrado."));
            _repositoryMock.Verify(r => r.GetByCodeAsync(code), Times.Once);
            _repositoryMock.Verify(r => r.IsAirportInUseAsync(It.IsAny<string>()), Times.Never);
            _repositoryMock.Verify(r => r.DeleteAirportAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void DeleteAsync_AirportNotInUse_HardDelete()
        {
            var code = "SJO";
            var airport = new Airport { CodeAirport = code, NameAirport = "Test" };
            _repositoryMock.Setup(r => r.GetByCodeAsync(code)).ReturnsAsync(airport);
            _repositoryMock.Setup(r => r.IsAirportInUseAsync(code)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DeleteAirportAsync(code, false))
                           .ReturnsAsync((true, "Aeropuerto eliminado permanentemente."));

            var result = _service.DeleteAsync(code).Result;

            Assert.That(result.success, Is.True);
            Assert.That(result.message, Is.EqualTo("Aeropuerto eliminado permanentemente."));
            _repositoryMock.Verify(r => r.IsAirportInUseAsync(code), Times.Once);
            _repositoryMock.Verify(r => r.DeleteAirportAsync(code, false), Times.Once);
        }

        [Test]
        public void DeleteAsync_AirportInUse_SoftDelete()
        {
            var code = "SJO";
            var airport = new Airport { CodeAirport = code, NameAirport = "Test" };
            _repositoryMock.Setup(r => r.GetByCodeAsync(code)).ReturnsAsync(airport);
            _repositoryMock.Setup(r => r.IsAirportInUseAsync(code)).ReturnsAsync(true);
            _repositoryMock.Setup(r => r.DeleteAirportAsync(code, true))
                           .ReturnsAsync((true, "Aeropuerto marcado como inactivo."));

            var result = _service.DeleteAsync(code).Result;

            Assert.That(result.success, Is.True);
            Assert.That(result.message, Is.EqualTo("Aeropuerto marcado como inactivo."));
            _repositoryMock.Verify(r => r.IsAirportInUseAsync(code), Times.Once);
            _repositoryMock.Verify(r => r.DeleteAirportAsync(code, true), Times.Once);
        }

        [Test]
        public void DeleteAsync_RepositoryFails_ReturnsError()
        {
            var code = "SJO";
            var airport = new Airport { CodeAirport = code, NameAirport = "Test" };
            _repositoryMock.Setup(r => r.GetByCodeAsync(code)).ReturnsAsync(airport);
            _repositoryMock.Setup(r => r.IsAirportInUseAsync(code)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.DeleteAirportAsync(code, false))
                           .ReturnsAsync((false, "Error al eliminar."));

            var result = _service.DeleteAsync(code).Result;

            Assert.That(result.success, Is.False);
            Assert.That(result.message, Is.EqualTo("Error al eliminar."));
        }
    }
}