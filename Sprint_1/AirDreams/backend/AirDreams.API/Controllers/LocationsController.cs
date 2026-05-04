using Microsoft.AspNetCore.Mvc;

namespace AirDreams.API.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private static readonly List<string> Countries = new()
        {
            "Costa Rica", "Estados Unidos", "España", "México",
            "Colombia", "Panamá", "Argentina", "Brasil"
        };

        private static readonly Dictionary<string, List<string>> CitiesByCountry = new()
        {
            { "Costa Rica", new() { "San José", "Liberia" } },
            { "Estados Unidos", new() { "Miami", "Nueva York", "Los Ángeles" } },
            { "España", new() { "Madrid", "Barcelona" } },
            { "México", new() { "Ciudad de México", "Cancún" } },
            { "Colombia", new() { "Bogotá", "Medellín" } },
            { "Panamá", new() { "Panamá" } },
            { "Argentina", new() { "Buenos Aires" } },
            { "Brasil", new() { "São Paulo", "Rio de Janeiro" } }
        };

        [HttpGet("countries")]
        public IActionResult GetCountries() => Ok(Countries);

        [HttpGet("cities")]
        public IActionResult GetCities([FromQuery] string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return BadRequest("Debe especificar un país.");
            return CitiesByCountry.TryGetValue(country, out var cities)
                ? Ok(cities)
                : Ok(new List<string>());
        }
    }
}