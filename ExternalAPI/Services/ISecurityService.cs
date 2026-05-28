using System.Text.Json;

namespace AirDreams.ExternalAPI.Services
{
    public interface ISecurityService
    {
        Task<(bool IsValid, JsonElement Response)> ValidateKeyAsync(string apiKey);
    }
}
