using AirDreams.API.DTOs;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Moq;

public class ReportServiceTests
{
    private Mock<IReportRepository> _reportRepository = null!;
    private ReportService _service = null!;

    [SetUp]
    public void Setup()
    {
        _reportRepository = new Mock<IReportRepository>();
        _service = new ReportService(_reportRepository.Object);
    }

    [Test]
    public async Task GetIncomeReportAsync_WhenYearIsProvided()
    {
        // Arrange
        var report = new List<IncomeReportDTO>
        {
            new IncomeReportDTO
            {
                Mes = "January 2026",
                Anio = 2026,
                MesNumero = 1,
                TotalIngresos = 1000
            }
        };

        _reportRepository
            .Setup(r => r.GetIncomeReportAsync(
                new DateTime(2026, 1, 1),
                new DateTime(2027, 1, 1),
                "SJO",
                "LAX",
                "AirDreams"))
            .ReturnsAsync(report);

        // Act
        var result = await _service.GetIncomeReportAsync(
            2026,
            null,
            null,
            "SJO",
            "LAX",
            "AirDreams");

        // Assert
        Assert.That(result, Is.EqualTo(report));
        _reportRepository.Verify(r => r.GetIncomeReportAsync(
            new DateTime(2026, 1, 1),
            new DateTime(2027, 1, 1),
            "SJO",
            "LAX",
            "AirDreams"), Times.Once);
    }

    [Test]
    public async Task GetIncomeReportAsync_WhenDatesAreProvided()
    {
        // Arrange
        var startDate = new DateTime(2026, 3, 10, 15, 30, 0);
        var endDate = new DateTime(2026, 3, 20, 8, 0, 0);

        _reportRepository
            .Setup(r => r.GetIncomeReportAsync(
                new DateTime(2026, 3, 10),
                new DateTime(2026, 3, 21),
                null,
                null,
                "Mushu Airlines"))
            .ReturnsAsync(new List<IncomeReportDTO>());

        // Act
        await _service.GetIncomeReportAsync(
            null,
            startDate,
            endDate,
            null,
            null,
            "Mushu Airlines");

        // Assert
        _reportRepository.Verify(r => r.GetIncomeReportAsync(
            new DateTime(2026, 3, 10),
            new DateTime(2026, 3, 21),
            null,
            null,
            "Mushu Airlines"), Times.Once);
    }

    [Test]
    public void GetIncomeReportAsync_ShouldThrowArgumentException_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var startDate = new DateTime(2026, 4, 10);
        var endDate = new DateTime(2026, 4, 9);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetIncomeReportAsync(
                null,
                startDate,
                endDate,
                null,
                null,
                null));
    }

    [Test]
    public async Task GetIncomeReportFiltersAsync_ShouldReturnFiltersFromRepository()
    {
        // Arrange
        var filters = new IncomeReportFiltersDTO
        {
            Origins = new[] { "SJO", "LAX" },
            Destinations = new[] { "MAD", "MIA" },
            Years = new[] { 2026, 2025 },
            Airlines = new[] { "AirDreams", "Mushu Airlines" }
        };

        _reportRepository
            .Setup(r => r.GetIncomeReportFiltersAsync())
            .ReturnsAsync(filters);

        // Act
        var result = await _service.GetIncomeReportFiltersAsync();

        // Assert
        Assert.That(result, Is.EqualTo(filters));
        _reportRepository.Verify(r => r.GetIncomeReportFiltersAsync(), Times.Once);
    }
}
