using Moq;
using NUnit.Framework;
using AirDreams.API.Models;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace Unitest_Aircraft
{
    public class CreateAircraftTest
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
        public void AddAircraft_returnSucess()
        {
            // Arrange
            var aircraft = new AircraftModel
            {
                AdminID = 1,
                Modelo = "BOEING-TEST",
                AircraftSize = "Grande",
                MaxWeight = 1000,
                Cant_Asientos_Fila_Firstclass = 2,
                Cant_Filas_Firstclass = 5,
                Cant_Asientos_Fila_Turista = 6,
                Cant_Filas_Turista = 20
            };

            _aircraftRepository
                .Setup(x => x.ExistsByModel("BOEING-TEST"))
                .Returns(false);

            _aircraftRepository
                .Setup(x => x.AddAircraft(aircraft))
                .Returns(true);

            // Act
            var result = _service.AddAircraft(aircraft);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }
    }
}