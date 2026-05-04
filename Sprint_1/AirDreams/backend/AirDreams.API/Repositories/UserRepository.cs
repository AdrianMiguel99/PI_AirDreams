using System.Data;
using AirDreams.API.DTOs;
using AirDreams.API.Models;
using AirDreams.API.Repositories.Interfaces;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        private UserDTO MapToUserDTO(dynamic user)
        {
            return new UserDTO
            {
                Id = user.employeeID,
                FullName = user.fullName,
                Email = user.email,
                Role = user.role
            };
        }

        public List<UserDTO> GetAll()
        {
            const string sql = @"
                SELECT 
                    ae.employeeID,
                    ae.nameEmployee + ' ' + ae.lastnames AS fullName,
                    ae.emailInternalUser AS email,
                    ae.role
                FROM AirlineEmployee ae;
            ";
            var dynamicUsers = _connection.Query(sql).ToList<dynamic>();
            List<UserDTO> result = new List<UserDTO>();
            foreach (var user in dynamicUsers)
            {
                result.Add(MapToUserDTO(user));
            }
            return result;
        }

        public List<UserDTO> Search(string searchTerm)
        {
            const string sql = @"
                SELECT 
                    ae.employeeID,
                    ae.nameEmployee + ' ' + ae.lastnames AS fullName,
                    ae.emailInternalUser AS email,
                    ae.role
                FROM AirlineEmployee ae
                WHERE 
                    ae.nameEmployee LIKE @searchTerm
                    OR ae.lastnames LIKE @searchTerm
                    OR ae.emailInternalUser LIKE @searchTerm;
            ";
            
            var dynamicUsers = _connection.Query(sql, new { searchTerm = $"%{searchTerm}%" }).ToList<dynamic>();
            List<UserDTO> result = new List<UserDTO>();
            foreach (var user in dynamicUsers)
            {
                result.Add(MapToUserDTO(user));
            }
            return result;
        }     
    }
}