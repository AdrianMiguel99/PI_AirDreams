using AirDreams.API.Models.Dtos;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/airports")]
    public class AirportsController : ControllerBase
    {
        private readonly IAirportService _airportService;

        public AirportsController(IAirportService airportService)
        {
            _airportService = airportService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AirportDto>>> GetAll()
        {
            var list = await _airportService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<AirportDto>> GetByCode(string code)
        {
            var airport = await _airportService.GetByCodeAsync(code.ToUpperInvariant());
            if (airport is null) return NotFound();
            return Ok(airport);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAirportDto dto, [FromHeader(Name = "Admin-ID")] byte adminId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _airportService.CreateAsync(dto, adminId);
                return CreatedAtAction(nameof(GetByCode), new { code = created.Code }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}