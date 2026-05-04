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
                    'Administrador' AS role
                FROM Admin a
                JOIN AirlineEmployee ae ON a.employeeID = ae.employeeID

                UNION ALL

                SELECT 
                    ae.employeeID,
                    ae.nameEmployee + ' ' + ae.lastnames AS fullName,
                    ae.emailInternalUser AS email,
                    'Operador' AS role
                FROM Operator o
                JOIN AirlineEmployee ae ON o.employeeID = ae.employeeID
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
                    CASE 
                        WHEN a.employeeID IS NOT NULL THEN 'Administrador'
                        ELSE 'Operador'
                    END AS role
                FROM AirlineEmployee ae
                LEFT JOIN Admin a 
                    ON ae.employeeID = a.employeeID
                LEFT JOIN Operator o 
                    ON ae.employeeID = o.employeeID
                WHERE 
                    ae.nameEmployee LIKE '%' + @searchTerm + '%'
                    OR ae.lastnames LIKE '%' + @searchTerm + '%'
                    OR ae.emailInternalUser LIKE '%' + @searchTerm + '%';
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