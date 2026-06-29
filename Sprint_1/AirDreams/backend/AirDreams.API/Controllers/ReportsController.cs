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
            [FromQuery] string? destination,
            [FromQuery] string? airline)
        {
            try
            {
                var report = await _reportService.GetIncomeReportAsync(
                    year,
                    startDate,
                    endDate,
                    origin,
                    destination,
                    airline);

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
    }
}
