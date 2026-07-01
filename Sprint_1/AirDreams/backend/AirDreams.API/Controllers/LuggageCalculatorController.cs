using Microsoft.AspNetCore.Mvc;
using AirDreams.API.DTOs;
using AirDreams.API.Services;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/luggage/calculate")]
    public class LuggageCalculatorController : ControllerBase
    {
        private readonly ILuggageCalculationService _service;

        public LuggageCalculatorController(ILuggageCalculationService service)
        {
            _service = service;
        }

        [HttpPost("total")]
        public async Task<IActionResult> CalculateTotal([FromBody] LuggageTotalRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.CalculateTotalsAsync(request);
            return Ok(result);
        }
    }
}