using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("income")]
        public async Task<IActionResult> GetIncomeReport(
            [FromQuery] int? year,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? origin,
            [FromQuery] string? destination)
        {
            try
            {
                var report = await _reportService.GetIncomeReportAsync(
                    year,
                    startDate,
                    endDate,
                    origin,
                    destination);

                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [HttpGet("income/filters")]
        public async Task<IActionResult> GetIncomeReportFilters()
        {
            var filters = await _reportService.GetIncomeReportFiltersAsync();
            return Ok(filters);
        }

        [HttpGet("flights")]
        public async Task<IActionResult> GetFlightsReport(
            [FromQuery] string? origin,
            [FromQuery] string? destination,
            [FromQuery] string? seatClass,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var report = await _reportService.GetFlightsReportAsync(
                    origin, destination, seatClass, fromDate, toDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("flights/filters")]
        public async Task<IActionResult> GetFlightsReportFilters()
        {
            var filters = await _reportService.GetFlightsReportFiltersAsync();
            return Ok(filters);
        }
    }
}
