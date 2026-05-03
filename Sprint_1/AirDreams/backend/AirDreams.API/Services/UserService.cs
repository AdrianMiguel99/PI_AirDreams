using AirDreams.API.DTOs;
using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<UserDTO> GetAll()
        {
            return userRepository.GetAll();
        }

        public List<UserDTO> Search(string searchTerm)
        {
            return userRepository.Search(searchTerm);
        }
    }
}
