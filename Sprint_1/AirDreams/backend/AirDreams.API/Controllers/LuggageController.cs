using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Models;
using AirDreams.API.Services.Interfaces;
using AirDreams.API.DTOs;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LuggageController : ControllerBase
    {
        private readonly ILuggageService _luggageService;

        public LuggageController(ILuggageService luggageService)
        {
            _luggageService = luggageService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterLuggage([FromBody] LuggageRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _luggageService.RegisterLuggageAsync(model);

            if (result.success)
            {
                return Ok(new { message = result.message });
            }

            return BadRequest(new { message = result.message });
        }

        [HttpPost("availability")]
        public async Task<IActionResult> CheckAvailability([FromBody] LuggageAvailabilityDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _luggageService.CheckAvailabilityAsync(dto.flightId, dto.LuggageWeight, dto.CarryOnWeight);

            if (result.success)
            {
                return Ok(new { success = true, message = result.message });
            }

            return BadRequest(new { success = false, message = result.message });
        }
    }
}