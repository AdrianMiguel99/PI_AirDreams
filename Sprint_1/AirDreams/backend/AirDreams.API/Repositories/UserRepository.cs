using AirDreams.API.Models;
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