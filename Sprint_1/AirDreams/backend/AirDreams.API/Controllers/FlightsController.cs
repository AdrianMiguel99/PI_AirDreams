using Microsoft.AspNetCore.Mvc;
using AirDreams.API.DTOs;
using AirDreams.API.Models.Dto;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFlights(
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] DateTime departureDate,
            [FromQuery] DateTime returnDate,
            [FromQuery] int passengers
        )
        {
            try
            {
                if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
                {
                    return BadRequest(new
                    {
                        code = 400,
                        description = "Origen y destino son requeridos"
                    });
                }

                if (passengers < 1)
                {
                    return BadRequest(new
                    {
                        code = 400,
                        description = "El número de pasajeros debe ser al menos 1"
                    });
                }

                var flights = await _flightService.SearchFlightsAsync(origin, destination, departureDate, returnDate, passengers);
                
                var response = new { flights };
                return Ok(response);
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
            {                return StatusCode(500, new
                {
                    code = 500,
                    description = "Ocurrió un error inesperado"
                });
            }
        }

        [HttpGet("search-destination")]
        public async Task<IActionResult> SearchFlightsByDestinationOnly([FromQuery] DestinationSearchDto request)
        {
            try
            {
                request.Validate();

                var flights = await _flightService.SearchFlightsByDestinationAsync(
                    request.Destination,
                    request.DepartureDate,
                    request.ReturnDate,
                    request.Passengers
                );
                
                var response = new { flights };
                return Ok(response);
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
                    description = "Ocurrió un error inesperado"
                });
            }
        }
    }
}
