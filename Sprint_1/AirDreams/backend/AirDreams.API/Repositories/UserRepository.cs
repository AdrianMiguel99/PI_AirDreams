using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories.Interfaces;

namespace AirDreams.API.Repositories
{
    public class UserRepository : IRepository<UserDTO>
    {
        private static readonly List<User> mockDatabaseData = new List<User>
        {
            new User
            {
                UserId = 1,
                Email = "juan.garcia@airdreams.com",
                EmployeeId = 1,
                FirstName = "Juan",
                LastName = "García",
                IsOperator = true
            },
            new User
            {
                UserId = 2,
                Email = "maria.rodriguez@airdreams.com",
                EmployeeId = 2,
                FirstName = "María",
                LastName = "Rodríguez",
                IsOperator = false
            },
            new User
            {
                UserId = 3,
                Email = "carlos.fernandez@airdreams.com",
                EmployeeId = 3,
                FirstName = "Carlos",
                LastName = "Fernández",
                IsOperator = true
            },
            new User
            {
                UserId = 4,
                Email = "ana.martinez@airdreams.com",
                EmployeeId = 4,
                FirstName = "Ana",
                LastName = "Martínez",
                IsOperator = false
            },
            new User
            {
                UserId = 5,
                Email = "luis.gonzalez@airdreams.com",
                EmployeeId = 5,
                FirstName = "Luis",
                LastName = "González",
                IsOperator = true
            }
        };

        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.EmployeeId,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Role = user.IsOperator ? "Operador" : "Administrador"
            };
        }

        public List<UserDTO> GetAll()
        {
            // TODO: Reemplazar con QUERY a db

            return mockDatabaseData
                .Select(MapToUserDTO)
                .ToList();
        }

        public UserDTO GetById(string id)
        {
            // TODO: Reemplazar con QUERY a db

            if (byte.TryParse(id, out byte employeeId))
            {
                var user = mockDatabaseData.FirstOrDefault(u => u.EmployeeId == employeeId);
                return user != null ? MapToUserDTO(user) : null;
            }
            return null;
        }
    }
}
