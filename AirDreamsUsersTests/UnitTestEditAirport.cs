using Moq;
using NUnit.Framework;
using AirDreams.API.DTOs;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace Test1
{
    public class UnitTestEditAirport
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
        public void UpdateAsync_ExistingAirport_UpdatesSuccessfully()
        {
            var code = "SJO";
            var dto = new UpdateAirportDTO
            {
                Name = "Aeropuerto Internacional Juan Santamaría"
            };

            var airportEntity = new Airport
            {
                CodeAirport = code,
                NameAirport = "Juan Santamaría", 
                City = "San José",
                Country = "Costa Rica"
            };

            _repositoryMock.Setup(r => r.GetByCodeAsync(code))
                           .ReturnsAsync(airportEntity);
            _repositoryMock.Setup(r => r.UpdateAsync(code, dto.Name))
                           .ReturnsAsync(true);

            Assert.DoesNotThrowAsync(() => _service.UpdateAsync(code, dto));

            _repositoryMock.Verify(r => r.GetByCodeAsync(code), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(code, dto.Name), Times.Once);
        }

        [Test]
        public void UpdateAsync_NonExistingAirport_ThrowsKeyNotFoundException()
        {
            var code = "XYZ";
            var dto = new UpdateAirportDTO { Name = "Nuevo Nombre" };

            _repositoryMock.Setup(r => r.GetByCodeAsync(code))
                           .ReturnsAsync((Airport?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateAsync(code, dto)
            );

            _repositoryMock.Verify(r => r.GetByCodeAsync(code), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}