using AirDreams.API.Models.Entities;

namespace AirDreams.API.Repositories
{
    public interface IPassengerRepository
    {
        Task<bool> ExistsAsync(int idPassenger, string passport);
        Task<Passenger?> GetByIdAsync(int idPassenger, string passport);
        Task<Passenger> CreateAsync(Passenger passenger);
    }
}
