namespace AirDreams.ExternalAPI.Services
{
    public interface IPurchaseService
    {
        Task<(int StatusCode, JsonElement Content)> OrderFlightAsync(ExternalOrderRequestDto request);
    }
}