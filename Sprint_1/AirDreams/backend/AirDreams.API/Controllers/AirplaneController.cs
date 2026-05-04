using AirDreams.API.Models;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirplaneController : ControllerBase
    {
        private readonly IAircraftService aircraftService;

        public AirplaneController(IAircraftService aircraftService)
        {
            this.aircraftService = aircraftService;
        }

        [HttpGet]
        public ActionResult<List<AircraftModel>> Get()
        {
            return Ok(aircraftService.GetAircrafts());
        }

        [HttpPost]
        public ActionResult<bool> AddAirplane([FromBody] AircraftModel aircraft)
        {
            if (aircraft == null)
            {
                return BadRequest("Datos inválidos.");
            }

            var result = aircraftService.AddAircraft(aircraft);

            if (string.IsNullOrEmpty(result))
            {
                return Ok(true);
            }

            return BadRequest(result);
        }

        [HttpDelete("{plateNumber}")]
        public IActionResult Delete(string plateNumber)
        {
            var result = aircraftService.DeleteAircraft(plateNumber);

            if (string.IsNullOrEmpty(result))
            {
                return Ok(true);
            }

            return BadRequest(result);
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
        public IActionResult Update(string plateNumber, [FromBody] AircraftModel aircraft)
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

            return BadRequest(result);
        }
    }
}