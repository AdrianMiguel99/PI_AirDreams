using AirDreams.API.DTOs;
using AirDreams.API.Models;
using Microsoft.AspNetCore.Authorization;
using AirDreams.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AirDreams.API.Controllers
{
    [Route("api/routes")]
    [ApiController]
    [Authorize(Roles = "Admin")]     
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
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (adminIdClaim == null)
                return Unauthorized("No se pudo identificar al administrador.");
            byte adminId = byte.Parse(adminIdClaim);


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