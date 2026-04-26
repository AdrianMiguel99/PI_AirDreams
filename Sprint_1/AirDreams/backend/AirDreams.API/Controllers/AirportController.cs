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
        private readonly IAirportService airportService;

        public AirportController(IAirportService airportService)
        {
            this.airportService = airportService;
        }

        // GET: api/Airport for list
        [HttpGet]
        public ActionResult<List<AirportDTO>> GetAll()
        {
            // TODO : Airport service and repository to get all airports
            var airports = airportService.GetAllAirports();
            if (airports == null || airports.Count == 0)
            {
                return NotFound("No airports found");
            }
            return Ok(airports);
        }

    }
}