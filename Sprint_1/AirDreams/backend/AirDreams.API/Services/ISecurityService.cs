namespace AirDreams.API.Services.Interfaces
{
    public interface ISecurityService
    {
        Task ValidateApiKeyAsync(string apiKey);
    }
}
