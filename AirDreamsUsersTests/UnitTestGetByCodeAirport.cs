using Moq;
using NUnit.Framework;
using AirDreams.API.DTOs;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace Test1
{
    public class UnitTestGetByCodeAirport
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
        public void GetByCodeAsync_ExistingAirport_ReturnsAirportDto()
        {
            var code = "SJO";
            var airportEntity = new Airport
            {
                CodeAirport = code,
                NameAirport = "Juan Santamaría",
                City = "San José",
                Country = "Costa Rica",
                TimeZone = "-06:00"
            };

            _repositoryMock.Setup(r => r.GetByCodeAsync(code))
                           .ReturnsAsync(airportEntity);

            var result = _service.GetByCodeAsync(code).Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Code, Is.EqualTo(code));
            Assert.That(result.Name, Is.EqualTo(airportEntity.NameAirport));
            _repositoryMock.Verify(r => r.GetByCodeAsync(code), Times.Once);
        }

        [Test]
        public void GetByCodeAsync_NonExistingAirport_ReturnsNull()
        {
            var code = "XYZ";
            _repositoryMock.Setup(r => r.GetByCodeAsync(code))
                           .ReturnsAsync((Airport?)null);

            var result = _service.GetByCodeAsync(code).Result;

            Assert.That(result, Is.Null);
            _repositoryMock.Verify(r => r.GetByCodeAsync(code), Times.Once);
        }
    }
}