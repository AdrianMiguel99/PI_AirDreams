using System.Text.Json;

namespace AirDreams.ExternalAPI.Services
{
    public class FlightService : IFlightService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FlightService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(int StatusCode, JsonElement Content)> SearchFlightAsync(
            string origin,
            string destination,
            DateTime earliestDeparture,
            DateTime latestDeparture,
            int quantityOfPassengers
        )
        {
            var client = _httpClientFactory.CreateClient("InternalAPI");

            var query = $"api/flights/search" +
                        $"?origin={Uri.EscapeDataString(origin)}" +
                        $"&destination={Uri.EscapeDataString(destination)}" +
                        $"&departureDate={earliestDeparture:O}" +
                        $"&returnDate={latestDeparture:O}" +
                        $"&passengers={quantityOfPassengers}";

            var flightsResponse = await client.GetAsync(query);
            var flightsContent = await flightsResponse.Content.ReadFromJsonAsync<JsonElement>();

            return ((int)flightsResponse.StatusCode, flightsContent);
        }
    }
}
