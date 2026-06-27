using AirDreams.API.DTOs;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/cancellation")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly ICancellationService _service;

        public CancellationController(ICancellationService service)
        {
            _service = service;
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelReservation(
            [FromBody] CancelReservationDto dto)
        {
            try
            {
                var result =
                    await _service.CancelReservationAsync(dto.TransactionId);

                return Ok(new
                {
                    success = result,
                    message = "Reserva cancelada correctamente."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}