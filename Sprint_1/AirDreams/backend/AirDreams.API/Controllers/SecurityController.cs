using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;

        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateApiKey([FromQuery] string apiKey)
        {
            try
            {
                await _securityService.ValidateApiKeyAsync(apiKey);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { description = ex.Message });
            }
        }
    }
}