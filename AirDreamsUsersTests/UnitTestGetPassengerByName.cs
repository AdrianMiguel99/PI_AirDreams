using Moq;
using NUnit.Framework;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace PassengerTests
{
    public class UnitTestGetPassengerByName
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
        public async Task GetPassengerByNameAsync_FullNameExistingPassenger_ReturnsPassengerDto()
        {
            var fullName = "Josue Ortiz Cabrera";
            var nameUsedByService = "Josue";

            var passengerEntity = new Passenger
            {
                IdPassenger = 99999,
                NamePassenger = "Josue",
                LastnamesPassenger = "Ortiz Cabrera",
                EmailPassenger = "Josue1223.ortiz@email.com",
                Telephone = 70110161,
                Country = "Costa Rica"
            };

            _repositoryMock
                .Setup(r => r.GetPassengerByNameAsync(nameUsedByService))
                .ReturnsAsync(passengerEntity);

            var result = await _service.GetPassengerByNameAsync(fullName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IdPassenger, Is.EqualTo(passengerEntity.IdPassenger));
            Assert.That(result.NamePassenger, Is.EqualTo(passengerEntity.NamePassenger));
            Assert.That(result.LastnamesPassenger, Is.EqualTo(passengerEntity.LastnamesPassenger));
            Assert.That(result.EmailPassenger, Is.EqualTo(passengerEntity.EmailPassenger));
            Assert.That(result.Telephone, Is.EqualTo(passengerEntity.Telephone));
            Assert.That(result.Country, Is.EqualTo(passengerEntity.Country));

            _repositoryMock.Verify(r => r.GetPassengerByNameAsync(nameUsedByService), Times.Once);
        }

        [Test]
        public async Task GetPassengerByNameAsync_OnlyOneName_ReturnsNull()
        {
            var fullName = "Josue";

            var result = await _service.GetPassengerByNameAsync(fullName);

            Assert.That(result, Is.Null);

            _repositoryMock.Verify(
                r => r.GetPassengerByNameAsync(It.IsAny<string>()),
                Times.Never
            );
        }

        [Test]
        public async Task GetPassengerByNameAsync_FullNameNonExistingPassenger_ReturnsNull()
        {
            var fullName = "No EXISTE";
            var nameUsedByService = "No";

            _repositoryMock
                .Setup(r => r.GetPassengerByNameAsync(nameUsedByService))
                .ReturnsAsync((Passenger?)null);

            var result = await _service.GetPassengerByNameAsync(fullName);

            Assert.That(result, Is.Null);

            _repositoryMock.Verify(r => r.GetPassengerByNameAsync(nameUsedByService), Times.Once);
        }
    }
}