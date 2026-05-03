using AirDreams.API.Models;
using Dapper;

namespace AirDreams.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string IDbconnection _dbConnection;

        public UserRepository(IDbconnection _db)
        {
            _dbConnection = _db;
        }

        public async Task<int> Create(RegisterUserModel user)
        {
            var sql = @"
            INSERT INTO Users (Nombre, Email, Cedula, TipoUsuario, EmailConfirmed)
            VALUES (@Nombre, @Email, @Cedula, @TipoUsuario, @EmailConfirmed);

            SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            var id = await _dbConnection.ExecuteAsync(sql, new
                {
                    Nombre = user.NombreCompleto,
                    Email = user.Email,
                    Cedula = user.Cedula,
                    TipoUsuario = user.TipoUsuario,
                    EmailConfirmed = user.emailConfirmed
                });

            return id;
        }

        public async Task<RegisterUserModel> GetByEmail(string email)
        {
            string query = "SELECT 
                            nombre,
                            email,
                            cedula
                            FROM dbo.Users
                            WHERE email = @email";

            return await _dbConnection.QueryFirstOrDefaultAsync<RegisterUserModel>(query, new { email });
        }
    }
}