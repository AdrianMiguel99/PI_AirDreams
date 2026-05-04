using AirDreams.API.DTOs;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private readonly IService<AircraftDTO> aircraftService;

        public AircraftController(IService<AircraftDTO> aircraftService)
        {
            this.aircraftService = aircraftService;
        }

        // GET: api/Aircraft
        [HttpGet]
        public ActionResult<List<AircraftDTO>> GetAll()
        {
            // TODO : Aircraft service and repository to get all aircrafts
            var aircrafts = aircraftService.GetAll();
            return Ok(aircrafts);
        }

        // GET: api/Aircraft/{id}
        [HttpGet("{id}")]
        public ActionResult<AircraftDTO> GetById(string id)
        {
            // TODO : Aircraft service and repository to get an aircraft by id
            var aircraft = aircraftService.GetById(id);
            if (aircraft == null)
            {
                return NotFound(new { message = "Aircraft not found" });
            }
            return Ok(aircraft);
        }

    }
}