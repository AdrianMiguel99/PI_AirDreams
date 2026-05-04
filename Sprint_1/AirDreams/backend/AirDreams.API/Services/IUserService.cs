using AirDreams.API.DTOs;

namespace AirDreams.API.Services.Interfaces
{
    public interface IUserService
    {
        List<UserDTO> GetAll();
        List<UserDTO> Search(string searchTerm);
    }
}
