using System.Text.Json;

namespace AirDreams.ExternalAPI.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SecurityService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(bool IsValid, JsonElement Response)> ValidateKeyAsync(string apiKey)
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
                return (false, error);
            }

            return (true, new JsonElement());
        }
    }
}
