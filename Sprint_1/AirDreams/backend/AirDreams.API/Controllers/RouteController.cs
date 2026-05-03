using AirDreams.API.DTOs;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IService<RouteDTO> routeService;

        public RouteController(IService<RouteDTO> routeService)
        {
            this.routeService = routeService;
        }

        // GET: api/Route
        [HttpGet]
        public ActionResult<List<RouteDTO>> GetAll()
        {
            // TODO : Route service and repository to get all routes
            var routes = routeService.GetAll();
            return Ok(routes);
        }

        // GET: api/Route/{id}
        [HttpGet("{id}")]
        public ActionResult<RouteDTO> GetById(string id)
        {
            // TODO : Route service and repository to get a route by id
            var route = routeService.GetById(id);
            if (route == null)
            {
                return NotFound(new { message = "Route not found" });
            }
            return Ok(route);
        }

    }
}