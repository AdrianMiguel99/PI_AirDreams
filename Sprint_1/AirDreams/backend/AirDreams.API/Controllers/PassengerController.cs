using AirDreams.API.DTOs;
using AirDreams.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/passengers")]
    public class PassengersController : ControllerBase
    {
        private readonly IPassengerService _passengerService;

        public PassengersController(IPassengerService passengerService)
        {
            _passengerService = passengerService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePassengerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _passengerService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        idPassenger = created.IdPassenger,
                        passport = created.Passport
                    },
                    created
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpGet("{idPassenger:int}/{passport}")]
        public async Task<IActionResult> GetById(int idPassenger, string passport)
        {
            var passenger = await _passengerService.GetByIdAsync(idPassenger, passport);

            if (passenger is null)
                return NotFound(new { error = "Pasajero no encontrado." });

            return Ok(passenger);
        }
    }
}
