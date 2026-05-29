using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IPassengerService
    {
        Task<PassengerDto> CreateAsync(CreatePassengerDto dto);
        Task<PassengerDto?> GetByIdAsync(int idPassenger, string passport);

        Task<PassengerDto?> GetPassengerByNameAsync(string fullName);
    }
}
