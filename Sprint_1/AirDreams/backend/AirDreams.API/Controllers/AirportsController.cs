using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<ActionResult<List<AirportDTO>>> GetAll()
        {
            var airports = await _airportService.GetAllAsync();
            return Ok(airports);
        }

        [HttpGet("{code}")]   
        public async Task<ActionResult<AirportDTO>> GetByCode(string code)
        {
            var airport = await _airportService.GetByCodeAsync(code.ToUpperInvariant());
            if (airport is null)
                return NotFound($"No se encontró el aeropuerto con código {code}.");
            return Ok(airport);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]   
        public async Task<IActionResult> Create([FromBody] CreateAirportDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminIdClaim == null)
                return Unauthorized("No se pudo identificar al administrador.");
            byte adminId = byte.Parse(adminIdClaim);

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

        [HttpPut("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string code, [FromBody] UpdateAirportDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _airportService.UpdateAsync(code.ToUpperInvariant(), dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                var (success, message) = await _airportService.DeleteAsync(code.ToUpperInvariant());
                if (!success)
                    return NotFound(new { error = message });
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}