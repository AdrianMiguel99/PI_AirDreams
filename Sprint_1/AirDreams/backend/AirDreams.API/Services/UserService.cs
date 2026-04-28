using AirDreams.API.DTOs;
using AirDreams.API.Repositories.Interfaces;
using AirDreams.API.Services.Interfaces;

namespace AirDreams.API.Services
{
    public class UserService : IService<UserDTO>
    {
        private readonly IRepository<UserDTO> userRepository;

        public UserService(IRepository<UserDTO> userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<UserDTO> GetAll()
        {
            return userRepository.GetAll();
        }

        public UserDTO GetById(string id)
        {
            return userRepository.GetById(id);
        }
    }
}
