using AirDreams.API.Models.Entities;

namespace AirDreams.API.Repositories
{
    public interface IPassengerRepository
    {
        Task<bool> ExistsAsync(int idPassenger);
        Task<Passenger?> GetByIdAsync(int idPassenger);
        Task<Passenger> CreateAsync(Passenger passenger);

        Task<Passenger> GetPassengerByNameAsync(string name);
    }
}
