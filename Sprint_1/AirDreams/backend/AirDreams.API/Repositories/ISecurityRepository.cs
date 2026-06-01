namespace AirDreams.API.Repositories
{
    public interface ISecurityRepository
    {
        Task<string?> ValidateApiKeyAsync(string apiKey);
    }
}
