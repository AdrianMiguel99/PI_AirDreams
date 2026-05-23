using AirDreams.API.Models;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin")]
        public ActionResult<bool> AddAirplane([FromBody] AircraftModel aircraft)
        {
            if (aircraft == null)
            {
                return BadRequest("Datos inválidos.");
            }

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //variable claim
            var result = aircraftService.AddAircraft(aircraft);

            // si es nula entonces NO es admin
            if (adminIdClaim == null)
            {
                return Unauthorized("No se pudo identificar el administrador.");
            }
            aircraft.AdminID = int.Parse(adminIdClaim);

            var result2 = aircraftService.AddAircraft(aircraft);

            if (string.IsNullOrEmpty(result2))
            {
                return Ok(true);
            }

            return BadRequest(result);
        }

        [HttpDelete("{modelo}")]
        public IActionResult Delete(string modelo)
        {
            var result = aircraftService.DeleteAircraft(modelo);

            if (string.IsNullOrEmpty(result))
            {
                return Ok(true);
            }

            return BadRequest(result);
        }

        [HttpGet("{modelo}")]
        public ActionResult<AircraftModel> GetAircraftByModel(string modelo)
        {
            var aircraft = aircraftService.GetAircraftByModel(modelo);

            if (aircraft == null)
            {
                return NotFound("Aeronave no encontrada.");
            }

            return Ok(aircraft);
        }

        [HttpPut("{modelo}")]
        public IActionResult Update(string modelo, [FromBody] AircraftModel aircraft)
        {
            if (aircraft == null || modelo != aircraft.Modelo   )
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