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
        public ActionResult AddAirplane([FromBody] AircraftModel aircraft)
        {
            if (aircraft == null)
            {
                return BadRequest(new
                {
                    message = "Los datos de la aeronave son inválidos."
                });
            }

            var adminIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (adminIdClaim == null)
            {
                return Unauthorized(new
                {
                    message = "No se pudo identificar el administrador."
                });
            }

            aircraft.AdminID = int.Parse(adminIdClaim);

            var result = aircraftService.AddAircraft(aircraft);

            if (string.IsNullOrEmpty(result))
            {
                return Ok(new
                {
                    message = "Aeronave registrada correctamente."
                });
            }

            return BadRequest(new
            {
                message = result
            });
        }

        [HttpDelete("{modelo}")]
        public IActionResult Delete(string modelo)
        {
            var result = aircraftService.DeleteAircraft(modelo);

            if (string.IsNullOrEmpty(result))
            {
                return Ok(new
                {
                    message = "Aeronave eliminada correctamente."
                });
            }

            return BadRequest(new
            {
                message = result
            });
        }

        [HttpGet("{modelo}")]
        public ActionResult<AircraftModel> GetAircraftByModel(string modelo)
        {
            var aircraft = aircraftService.GetAircraftByModel(modelo);

            if (aircraft == null)
            {
                return NotFound(new
                {
                    message = "Aeronave no encontrada."
                });
            }

            return Ok(aircraft);
        }

        [HttpPut("{modelo}")]
        public IActionResult Update(string modelo, [FromBody] AircraftModel aircraft)
        {
            if (aircraft == null || modelo != aircraft.Modelo)
            {
                return BadRequest(new
                {
                    message = "Los datos enviados no coinciden con la aeronave seleccionada."
                });
            }

            var result = aircraftService.UpdateAircraft(aircraft);

            if (string.IsNullOrEmpty(result))
            {
                return Ok(new
                {
                    message = "Aeronave actualizada correctamente."
                });
            }

            return BadRequest(new
            {
                message = result
            });
        }
    }
}