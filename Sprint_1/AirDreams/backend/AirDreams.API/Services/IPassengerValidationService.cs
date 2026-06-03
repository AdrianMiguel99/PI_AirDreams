using AirDreams.API.Repositories;  

namespace AirDreams.API.Services
{
    public interface IPassengerValidationService
    {
        Task<IEnumerable<string>> ValidateAsync(
            IEnumerable<PassengerCheck> passengers,
            IEnumerable<string> flightNumbers);
    }
}