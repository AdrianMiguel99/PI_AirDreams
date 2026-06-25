using AirDreams.API.DTOs;
using AirDreams.API.Models;
using Microsoft.AspNetCore.Authorization;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AirDreams.API.Controllers
{
    [Route("api/routes")]
    [ApiController]
    [Authorize(Roles = "Admin")]     
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly IAirportRepository _airportRepository;

        public RouteController(IRouteService routeService, IAirportRepository airportRepository)
        {
            _routeService = routeService;
            _airportRepository = airportRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool originActive = await _airportRepository.IsActiveAsync(model.CodeAirportSalida);
            bool destinationActive = await _airportRepository.IsActiveAsync(model.CodeAirportLlegada);
            if (!originActive || !destinationActive)
                return BadRequest(new { message = "Uno o ambos aeropuertos están inactivos y no pueden usarse para nuevas rutas." });

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (adminIdClaim == null)
                return Unauthorized("No se pudo identificar al administrador.");
            
            byte adminId = byte.Parse(adminIdClaim);


            try{

                var id = await _routeService.CreateRouteWithFrequenciesAsync(model, adminId);

                return CreatedAtAction(nameof(Create), new { id }, new { RouteID = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {message = ex.Message});
            }

            
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<RouteDTO>>> GetAllAsync()
        {
            var routes = await _routeService.GetAllAsync();
            return Ok(routes);
        }

    }
}