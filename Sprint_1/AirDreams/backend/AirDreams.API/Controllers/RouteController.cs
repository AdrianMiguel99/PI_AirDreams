using AirDreams.API.DTOs;
using AirDreams.API.Models;
using Microsoft.AspNetCore.Authorization;
using AirDreams.API.Repositories;
using AirDreams.API.Services;
using Microsoft.Data.SqlClient;
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

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _routeService.DeleteAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
