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
        public async Task<ActionResult<bool>> AddAirplane(AircraftModel aircraft)
        {
            if (aircraft == null)
            {
                return BadRequest();
            }

            var result = aircraftService.AddAircraft(aircraft);
            if (string.IsNullOrEmpty(result))
            {
                return Ok(true);

            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}