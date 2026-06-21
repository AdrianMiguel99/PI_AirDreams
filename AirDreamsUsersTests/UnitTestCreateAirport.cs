using Moq;
using NUnit.Framework;                       
using AirDreams.API.Models.Dtos;
using AirDreams.API.Models.Entities;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace Test1
{
    public class UnitTestCreateAirport
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
        public void CreateAsync_ValidAirport_ReturnsAirportDto()
        {
            var dto = new CreateAirportDto
            {
                Code = "MAD",
                Name = "Barajas",
                City = "Madrid",
                Country = "España",
                TimeZone = "01:00:00"       
            };
            byte adminId = 1;

            _repositoryMock.Setup(r => r.ExistsAsync(dto.Code))
                           .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Airport>()))
                           .ReturnsAsync(new Airport
                           {
                               CodeAirport = dto.Code,
                               NameAirport = dto.Name,
                               City = dto.City,
                               Country = dto.Country,
                               TimeZone = dto.TimeZone,  
                               AdminID = adminId
                           });

            var result = _service.CreateAsync(dto, adminId).Result;

            Assert.That(result.Code, Is.EqualTo(dto.Code));
            Assert.That(result.Name, Is.EqualTo(dto.Name));
            _repositoryMock.Verify(r => r.ExistsAsync(dto.Code), Times.Once);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Airport>()), Times.Once);
        }

        [Test]
        public void CreateAsync_DuplicateCode_ThrowsInvalidOperationException()
        {
            var dto = new CreateAirportDto
            {
                Code = "MAD",
                Name = "Barajas",
                City = "Madrid",
                Country = "España"
            };
            byte adminId = 1;

            _repositoryMock.Setup(r => r.ExistsAsync(dto.Code))
                           .ReturnsAsync(true);

            Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateAsync(dto, adminId)
            );
        }
    }
}