using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/routes")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteRepository _routeRepository;

        public RouteController(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteModel model)
        {

            // dentro de Create(...)
            Console.WriteLine($"CodeAirportSalida='{model.CodeAirportSalida}', CodeAirportLlegada='{model.CodeAirportLlegada}'");

            if (model == null)
            {
                return BadRequest(new { message = "Route data is required" });
            }

            if (model.CodeAirportSalida == model.CodeAirportLlegada)
            {
                return BadRequest(new { message = "Origin and destination must differ" });
            }

            var id = await _routeRepository.CreateRouteWithFrequenciesAsync(model);

            return CreatedAtAction(nameof(Create), new { id }, new { RouteID = id });
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<RouteDTO>>> GetAllAsync()
        {
            var routes = await _routeRepository.GetAllAsync();
            return Ok(routes);
        }

    }
}