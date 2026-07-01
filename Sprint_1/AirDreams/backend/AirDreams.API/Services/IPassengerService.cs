using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IPassengerService
    {
        Task<PassengerDTO> CreateAsync(CreatePassengerDTO dto);
        Task<PassengerDTO?> GetByIdAsync(int idPassenger);

        Task<PassengerDTO?> GetPassengerByNameAsync(string fullName);
    }
}
