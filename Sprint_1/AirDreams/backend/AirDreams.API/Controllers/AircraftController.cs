using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private readonly AircraftService aircraftService;

        public AircraftController(AircraftService aircraftService)
        {
            this.aircraftService = aircraftService;
        }

        // GET: api/Aircraft for list
        [HttpGet]
        public ActionResult<List<AircraftDTO>> GetAll()
        {
            // TODO : Aircraft service and repository to get all aircrafts
            var aircrafts = aircraftService.GetAllAircrafts();
            return Ok(aircrafts);
        }

        // GET: api/Aircraft/{id}
        [HttpGet("{id}")]
        public ActionResult<AircraftDTO> GetById(string id)
        {
            // TODO : Aircraft service and repository to get an aircraft by id
            var aircraft = aircraftService.GetAircraftById(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }
            return Ok(aircraft);
        }

    }
}