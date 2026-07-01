using Moq;
using NUnit.Framework;
using AirDreams.API.DTOs;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace Test1
{
    public class UnitTestFlightsReport
    {
        private Mock<IReportRepository> _reportRepositoryMock;
        private ReportService _service;

        [SetUp]
        public void Setup()
        {
            _reportRepositoryMock = new Mock<IReportRepository>();
            _service = new ReportService(_reportRepositoryMock.Object);
        }

        [Test]
        public void GetFlightsReport_WithFilters_ReturnsFilteredData()
        {
            var expectedData = new List<FlightsReportRowDTO>
            {
                new FlightsReportRowDTO
                {
                    FlightDate = new DateTime(2026, 6, 25),
                    Origin = "SJO",
                    Destination = "CDG",
                    NumberFlight = "AD12202606",
                    Airline = "AirDreams",
                    FirstClassPassengers = 5,
                    TouristPassengers = 30,
                    PassengerRevenue = 25000m,
                    LuggageRevenue = 1500m,
                    TotalRevenue = 26500m
                }
            };

            _reportRepositoryMock
                .Setup(r => r.GetFlightsReportAsync("SJO", "CDG", "FirstClass", It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(expectedData);

            var result = _service.GetFlightsReportAsync("SJO", "CDG", "FirstClass", new DateTime(2026, 6, 1), new DateTime(2026, 6, 30)).Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Origin, Is.EqualTo("SJO"));
            _reportRepositoryMock.Verify(r => r.GetFlightsReportAsync("SJO", "CDG", "FirstClass", It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
        }

        [Test]
        public void GetFlightsReport_NoFilters_ReturnsAllData()
        {
            var allData = new List<FlightsReportRowDTO>
            {
                new FlightsReportRowDTO { NumberFlight = "AD001" },
                new FlightsReportRowDTO { NumberFlight = "AD002" }
            };

            _reportRepositoryMock
                .Setup(r => r.GetFlightsReportAsync(null, null, null, null, null))
                .ReturnsAsync(allData);

            var result = _service.GetFlightsReportAsync(null, null, null, null, null).Result;

            Assert.That(result.Count(), Is.EqualTo(2));
            _reportRepositoryMock.Verify(r => r.GetFlightsReportAsync(null, null, null, null, null), Times.Once);
        }

        [Test]
        public void GetFlightsReportFilters_ReturnsFilters()
        {
            var filters = new FlightsReportFiltersDTO
            {
                Origins = new List<string> { "SJO", "CDG" },
                Destinations = new List<string> { "CDG", "JFK" },
                MinDate = new DateTime(2026, 1, 1),
                MaxDate = new DateTime(2026, 12, 31)
            };

            _reportRepositoryMock
                .Setup(r => r.GetFlightsReportFiltersAsync())
                .ReturnsAsync(filters);

            var result = _service.GetFlightsReportFiltersAsync().Result;

            Assert.That(result.Origins.Count(), Is.EqualTo(2));
            Assert.That(result.Destinations.Count(), Is.EqualTo(2));
            Assert.That(result.MinDate, Is.EqualTo(new DateTime(2026, 1, 1)));
            Assert.That(result.MaxDate, Is.EqualTo(new DateTime(2026, 12, 31)));
            _reportRepositoryMock.Verify(r => r.GetFlightsReportFiltersAsync(), Times.Once);
        }

        [Test]
        public void GetFlightsReport_RepositoryThrowsException_PropagatesError()
        {
            _reportRepositoryMock
                .Setup(r => r.GetFlightsReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ThrowsAsync(new Exception("Error de base de datos"));

            Assert.ThrowsAsync<Exception>(() => _service.GetFlightsReportAsync(null, null, null, null, null));
            _reportRepositoryMock.Verify(r => r.GetFlightsReportAsync(null, null, null, null, null), Times.Once);
        }
    }
}