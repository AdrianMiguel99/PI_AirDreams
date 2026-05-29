using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AirDreams.ExternalAPI.Services;

namespace AirDreams.ExternalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly ISecurityService _securityService;

        public ExternalController(IFlightService flightService, ISecurityService securityService)
        {
            _flightService = flightService;
            _securityService = securityService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchFlights(
            [FromQuery] string destination,
            [FromQuery] DateTime earliestDeparture,
            [FromQuery] DateTime latestDeparture,
            [FromQuery] int quantityOfPassengers,
            [FromQuery] string? apiKey
        )
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return Unauthorized(new { code = "401", description = "INVALID_API_KEY: La API key es requerida." });

            try
            {
                var (isValid, errorResponse) = await _securityService.ValidateKeyAsync(apiKey);

                if (!isValid)
                {
                    return Unauthorized(errorResponse);
                }

                var (statusCode, flightsContent) = await _flightService.SearchFlightAsync(destination, earliestDeparture, latestDeparture, quantityOfPassengers);

                return StatusCode(statusCode, flightsContent);
            }
            catch (Exception)
            {
                return StatusCode(500, new { code = "500", description = "Ha ocurrido un error inesperado" });
            }
        }
    }
}