using Moq;
using NUnit.Framework;
using AirDreams.API.DTOs;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace PassengerTests
{
    public class UnitTestCreatePassenger
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
        public async Task CreateAsync_ValidPassenger_ReturnsPassengerdto()
        {
            var dto = new CreatePassengerDTO
            {
                IdPassenger = 99999,
                NamePassenger = "Jouse",
                LastnamesPassenger = "Ortiz Cabrera",
                EmailPassenger = "Josue1223.ortiz@gmail.com",
                Telephone = 70110161,
                Country = "Costa Rica"
            };
        

            _repositoryMock
                .Setup(r => r.ExistsAsync(dto.IdPassenger))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Passenger>()))
                .ReturnsAsync((Passenger passenger) => passenger);

            var result = await _service.CreateAsync(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IdPassenger, Is.EqualTo(dto.IdPassenger));
            Assert.That(result.NamePassenger, Is.EqualTo(dto.NamePassenger));
            Assert.That(result.LastnamesPassenger, Is.EqualTo(dto.LastnamesPassenger));
            Assert.That(result.EmailPassenger, Is.EqualTo(dto.EmailPassenger?.ToLower()));
            Assert.That(result.Telephone, Is.EqualTo(dto.Telephone));
            Assert.That(result.Country, Is.EqualTo(dto.Country));

            _repositoryMock.Verify(r => r.ExistsAsync(dto.IdPassenger), Times.Once);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Passenger>()), Times.Once);

        }

        [Test]
        public void CreateAsync_DuplicatePassenger_ThrowsInvalidOperationException()
        {
            var dto = new CreatePassengerDTO
            {
                IdPassenger = 99999,
                NamePassenger = "Josue",
                LastnamesPassenger = "Ortiz Cabrera",
                EmailPassenger = "Josue1223.ortiz@gmail.com",
                Telephone = 70110161,
                Country = "Costa Rica"
            };

            _repositoryMock
                .Setup(r => r.ExistsAsync(dto.IdPassenger))
                .ReturnsAsync(true);

            Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(dto)
            );

            _repositoryMock.Verify(r => r.ExistsAsync(dto.IdPassenger), Times.Once);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Passenger>()), Times.Never);
        }

    }
}
