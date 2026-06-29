using System.Text.Json;

namespace AirDreams.ExternalAPI.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PurchaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<(int StatusCode, JsonElement Content)> OrderFlightAsync(ExternalOrderRequestDto request)
        {
            var client = _httpClientFactory.CreateClient("InternalAPI");

            var response = await client.PostAsJsonAsync(
                "api/payment/external-purchase",
                request);

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            return ((int)response.StatusCode, content);
        }
    }
}