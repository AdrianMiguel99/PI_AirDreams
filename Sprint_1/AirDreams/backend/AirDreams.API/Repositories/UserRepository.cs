using AirDreams.API.Models;
using AirDreams.API.DTOs;
using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace AirDreams.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
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
            var dynamicUsers = _dbConnection.Query(sql).ToList<dynamic>();
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
            
            var dynamicUsers = _dbConnection.Query(sql, new { searchTerm = $"%{searchTerm}%" }).ToList<dynamic>();
            List<UserDTO> result = new List<UserDTO>();
            foreach (var user in dynamicUsers)
            {
                result.Add(MapToUserDTO(user));
            }
            return result;
        }     

        public async Task<int> CreateInvitation(string email, string tipoUsuario, string token, DateTime expiryDate)
        {
            var sql = @"
                INSERT INTO Users (Email, TipoUsuario, InvitationToken, InvitationExpiryDate, IsActive)
                VALUES (@Email, @TipoUsuario, @Token, @ExpiryDate, 0);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var id = await _dbConnection.QuerySingleAsync<int>(sql, new
            {
                Email = email,
                TipoUsuario = tipoUsuario,
                Token = token,
                ExpiryDate = expiryDate
            });

            return id;
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { Email = email });
        }

        public async Task<InvitationData?> GetInvitationByToken(string token)
        {
            var sql = @"
                SELECT Email, TipoUsuario, InvitationToken AS Token, InvitationExpiryDate AS ExpiryDate
                FROM Users
                WHERE InvitationToken = @Token AND IsActive = 0
            ";

            return await _dbConnection.QueryFirstOrDefaultAsync<InvitationData>(sql, new { Token = token });
        }

        public async Task<bool> CompleteRegistration(string token, string nombreCompleto, string cedula, string passwordHash)
        {
            var sql = @"
                UPDATE Users 
                SET Nombre = @NombreCompleto,
                    Cedula = @Cedula,
                    PasswordHash = @PasswordHash,
                    IsActive = 1,
                    CompletedAt = GETDATE(),
                    InvitationToken = NULL,
                    InvitationExpiryDate = NULL
                WHERE InvitationToken = @Token AND IsActive = 0
            ";

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new
            {
                Token = token,
                NombreCompleto = nombreCompleto,
                Cedula = cedula,
                PasswordHash = passwordHash
            });

            return rowsAffected > 0;
        }

        public async Task<UserModel?> GetUserByEmail(string email)
        {
            var sql = @"
                SELECT Id, Nombre as NombreCompleto, Email as Correo, Cedula, TipoUsuario, IsActive, PasswordHash, CreatedAt
                FROM Users
                WHERE Email = @Email
            ";

            return await _dbConnection.QueryFirstOrDefaultAsync<UserModel>(sql, new { Email = email });
        }

        public async Task<UserModel?> GetUserById(int id)
        {
            var sql = @"
                SELECT Id, Nombre as NombreCompleto, Email as Correo, Cedula, TipoUsuario, IsActive, PasswordHash, CreatedAt
                FROM Users
                WHERE Id = @Id
            ";

            return await _dbConnection.QueryFirstOrDefaultAsync<UserModel>(sql, new { Id = id });
        }
    }
}