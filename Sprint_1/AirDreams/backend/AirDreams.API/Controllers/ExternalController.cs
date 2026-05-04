using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public ExternalController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchFlights(
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] string earliestDeparture,
            [FromQuery] string latestDeparture,
            [FromQuery] int quantityOfPassengers,
            [FromQuery] string apiKey
        )
        {
            try
            {
                await _flightService.ValidateApiKeyAsync(apiKey);
                var flights = await _flightService.SearchFlightsAsync(origin, destination, earliestDeparture, latestDeparture, quantityOfPassengers);
                var response = new { flights };
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    code = 401,
                    description = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    code = 400,
                    description = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = 500,
                    description = "Ha ocurrido un error inesperado"
                });
            }
        }
    }
}
