using AirDreams.API.DTOs;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IService<AirportDTO> airportService;

        public AirportController(IService<AirportDTO> airportService)
        {
            this.airportService = airportService;
        }

        // GET: api/Airport
        [HttpGet]
        public ActionResult<List<AirportDTO>> GetAll()
        {
            // TODO : Airport service and repository to get all airports
            var airports = airportService.GetAll();
            return Ok(airports);
        }

        // GET: api/Airport/{id}
        [HttpGet("{id}")]
        public ActionResult<AirportDTO> GetById(string id)
        {
            // TODO : Airport service and repository to get an airport by id
            var airport = airportService.GetById(id);
            if (airport == null)
            {
                return NotFound(new { message = "Airport not found" });
            }
            return Ok(airport);
        }

    }
}