using AirDreams.API.Models;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirplaneController : ControllerBase
    {
        private readonly AircraftService aircraftService;

        public AirplaneController(AircraftService aircraftService)
        {
            this.aircraftService = aircraftService;
        }

        [HttpGet]
        public List<AircraftModel> Get()
        {
            return aircraftService.GetAircrafts();
        }

        [HttpPost]
        public IActionResult Post([FromBody] AircraftModel aircraft)
        {
            aircraftService.AddAircraft(aircraft);
            return Ok("Aeronave registrada correctamente");
        }
    }
}