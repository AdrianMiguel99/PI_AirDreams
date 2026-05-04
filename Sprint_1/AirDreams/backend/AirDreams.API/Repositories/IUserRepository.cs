using AirDreams.API.DTOs;

namespace AirDreams.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<UserDTO> GetAll();
        List<UserDTO> Search(string searchTerm);
    }
}
