using Moq;
using NUnit.Framework;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace PassengerTests
{
    public class UnitTestGetPassengerById
    {
        private Mock<IPassengerRepository> _repositoryMock;
        private PassengerService _service;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IPassengerRepository>();
            _service = new PassengerService(_repositoryMock.Object);
        }

        [Test]
        public async Task GetByIdAsync_ExistingPassenger_ReturnsPassengerDto()
        {
            var idPassenger = 99999;

            var passengerEntity = new Passenger
            {
                IdPassenger = idPassenger,
                NamePassenger = "Josue",
                LastnamesPassenger = "Ortiz Cabrera",
                EmailPassenger = "Josue1223.ortiz@email.com",
                Telephone = 70110161,
                Country = "Costa Rica"
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(idPassenger))
                .ReturnsAsync(passengerEntity);

            var result = await _service.GetByIdAsync(idPassenger);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IdPassenger, Is.EqualTo(idPassenger));
            Assert.That(result.NamePassenger, Is.EqualTo(passengerEntity.NamePassenger));
            Assert.That(result.LastnamesPassenger, Is.EqualTo(passengerEntity.LastnamesPassenger));
            Assert.That(result.EmailPassenger, Is.EqualTo(passengerEntity.EmailPassenger));
            Assert.That(result.Telephone, Is.EqualTo(passengerEntity.Telephone));
            Assert.That(result.Country, Is.EqualTo(passengerEntity.Country));

            _repositoryMock.Verify(r => r.GetByIdAsync(idPassenger), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_NonExistingPassenger_ReturnsNull()
        {
            var idPassenger = 999999999;

            _repositoryMock
                .Setup(r => r.GetByIdAsync(idPassenger))
                .ReturnsAsync((Passenger?)null);

            var result = await _service.GetByIdAsync(idPassenger);

            Assert.That(result, Is.Null);

            _repositoryMock.Verify(r => r.GetByIdAsync(idPassenger), Times.Once);
        }
    }
}