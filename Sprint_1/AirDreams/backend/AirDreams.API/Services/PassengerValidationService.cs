using AirDreams.API.Repositories;

namespace AirDreams.API.Services
{
    public class PassengerValidationService : IPassengerValidationService
    {
        private readonly IPassengerValidationRepository _repository;

        public PassengerValidationService(IPassengerValidationRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<string>> ValidateAsync(
            IEnumerable<PassengerCheck> passengers,
            IEnumerable<string> flightNumbers)
        {
            return _repository.FindDuplicatePassengersAsync(passengers, flightNumbers);
        }
    }
}