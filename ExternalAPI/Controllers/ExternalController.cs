using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AirDreams.ExternalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> SearchFlights(
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] DateTime earliestDeparture,
            [FromQuery] DateTime latestDeparture,
            [FromQuery] int quantityOfPassengers,
            [FromQuery] string? apiKey
        )
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return Unauthorized(new { code = "401", description = "INVALID_API_KEY: La API key es requerida." });

            try
            {
                var client = _httpClientFactory.CreateClient("InternalAPI");

                var validationRequest = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"api/security/validate?apiKey={Uri.EscapeDataString(apiKey)}"
                );
                var validationResponse = await client.SendAsync(validationRequest);

                if (!validationResponse.IsSuccessStatusCode)
                {
                    var error = await validationResponse.Content.ReadFromJsonAsync<JsonElement>();
                    return Unauthorized(error);
                }

                var query = $"api/flights/search" +
                            $"?origin={Uri.EscapeDataString(origin)}" +
                            $"&destination={Uri.EscapeDataString(destination)}" +
                            $"&departureDate={earliestDeparture:O}" +
                            $"&returnDate={latestDeparture:O}" +
                            $"&passengers={quantityOfPassengers}";

                var flightsResponse = await client.GetAsync(query);
                var flightsContent = await flightsResponse.Content.ReadFromJsonAsync<JsonElement>();

                if (!flightsResponse.IsSuccessStatusCode)
                    return StatusCode((int)flightsResponse.StatusCode, flightsContent);

                return Ok(flightsContent);
            }
            catch (Exception)
            {
                return StatusCode(500, new { code = "500", description = "Ha ocurrido un error inesperado" });
            }
        }
    }
}