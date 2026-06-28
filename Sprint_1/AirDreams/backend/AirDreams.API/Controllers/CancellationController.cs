using AirDreams.API.DTOs;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/cancellations")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly ICancellationService _service;

        public CancellationController(ICancellationService service)
        {
            _service = service;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestCancellation(
            [FromBody] CancelReservationDto dto)
        {
            try
            {
                await _service.RequestCancellationAsync(dto.TransactionId);

                return Ok(new
                {
                    message = "Se ha enviado un correo de confirmación."
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

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmCancellation(
            [FromBody] ConfirmCancellationDto dto)
        {
            try
            {
                await _service.ConfirmCancellationAsync(dto.Token);

                return Ok(new
                {
                    message = "La reserva ha sido cancelada correctamente."
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

        [HttpPost]
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