using AirDreams.API.DTOs;
using AirDreams.API.Services;
using AirDreams.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService reservationService;

        public ReservationController(IReservationService reservationService)
        {
            this.reservationService = reservationService;
        }

        [HttpGet("{reservationCode}")]
        public async Task<ActionResult<ReservationDetailsDTO>> GetReservationDetails(string reservationCode)
        {
            if (string.IsNullOrWhiteSpace(reservationCode))
            {
                return BadRequest(new
                {
                    message = "El código de reserva es inválido."
                });
            }

            var reservationDetails = await reservationService.GetReservationDetailsAsync(reservationCode);

            if (reservationDetails == null)
            {
                return NotFound(new
                {
                    message = "Reserva no encontrada."
                });
            }

            return Ok(reservationDetails);
        }
    }
}