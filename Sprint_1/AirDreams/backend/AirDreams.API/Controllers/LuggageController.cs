using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Models;
using AirDreams.API.Services.Interfaces;

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
    }
}