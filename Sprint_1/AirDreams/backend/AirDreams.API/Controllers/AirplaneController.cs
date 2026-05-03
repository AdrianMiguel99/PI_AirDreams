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

        [HttpDelete("{plateNumber}")]
        public IActionResult Delete(string plateNumber)
        {

            var result = aircraftService.DeleteAircraft(plateNumber);

            if (string.IsNullOrEmpty(result))
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(result);
            }

        }

        [HttpGet("{plateNumber}")]
        public ActionResult<AircraftModel> GetByPlateNumber(string plateNumber)
        {
            var aircraft = aircraftService.GetAircraftByPlateNumber(plateNumber);

            if (aircraft == null)
            {
                return NotFound("Aeronave no encontrada.");
            }

            return Ok(aircraft);
        }

        [HttpPut("{plateNumber}")]
        public IActionResult Update(string plateNumber, AircraftModel aircraft)
        {
            if (aircraft == null || plateNumber != aircraft.plateNumber)
            {
                return BadRequest("Datos inválidos.");
            }

            var result = aircraftService.UpdateAircraft(aircraft);

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