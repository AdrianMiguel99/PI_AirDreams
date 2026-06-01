using AirDreams.API.Models;

namespace AirDreams.API.Services.Interfaces
{
    public interface ILuggageService
    {
        Task<(bool success, string message)> RegisterLuggageAsync(LuggageRegistrationModel model);
    }
}
