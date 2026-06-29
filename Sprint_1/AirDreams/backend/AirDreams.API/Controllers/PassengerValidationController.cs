using Microsoft.AspNetCore.Mvc;
using AirDreams.API.Repositories;
using AirDreams.API.Services;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/passengers/validate-duplicate")]
    public class PassengerValidationController : ControllerBase
    {
        private readonly IPassengerValidationService _service;

        public PassengerValidationController(IPassengerValidationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Validate([FromBody] ValidateRequest request)
        {
            var duplicates = await _service.ValidateAsync(
                request.Passengers.Select(p => new PassengerCheck
                {
                    NamePassenger = p.NamePassenger,
                    LastnamesPassenger = p.LastnamesPassenger,
                    Country = p.Country,
                    BirthDate = p.BirthDate
                }),
                request.FlightNumbers);

            return Ok(new { duplicates });
        }
    }

    public class ValidateRequest
    {
        public List<PassengerData> Passengers { get; set; }
        public List<string> FlightNumbers { get; set; }
    }

    public class PassengerData
    {
        public string NamePassenger { get; set; }
        public string LastnamesPassenger { get; set; }
        public string Country { get; set; }
        public string? BirthDate { get; set; }
    }
}