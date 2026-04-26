using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly AirportService airportService;

        public AirportController(AirportService airportService)
        {
            this.airportService = airportService;
        }

        // GET: api/Airport
        [HttpGet]
        public ActionResult<List<AirportDTO>> GetAll()
        {
            // TODO : Airport service and repository to get all airports
            var airports = airportService.GetAllAirports();
            return Ok(airports);
        }

        // GET: api/Airport/{id}
        [HttpGet("{id}")]
        public ActionResult<AirportDTO> GetById(string id)
        {
            // TODO : Airport service and repository to get an airport by id
            var airport = airportService.GetAirportById(id);
            if (airport == null)
            {
                return NotFound(new { message = "Airport not found" });
            }
            return Ok(airport);
        }

    }
}