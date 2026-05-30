using AirDreams.API.Models;
using AirDreams.API.Models.Entities;
using AirDreams.API.DTOs;
using Dapper;
using System.Data;
using System.Text;
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
            iu.emailUser AS email,
            CASE
                WHEN ae.isAdmin = 1 THEN 'Admin'
                WHEN ae.isOperator = 1 THEN 'Operator'
                ELSE 'User'
            END AS role
        FROM AirlineEmployee ae
        INNER JOIN InternalUser iu
            ON iu.emailUser = ae.emailInternalUser;
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
            iu.emailUser AS email,
            CASE
                WHEN ae.isAdmin = 1 THEN 'Admin'
                WHEN ae.isOperator = 1 THEN 'Operator'
                ELSE 'User'
            END AS role
        FROM AirlineEmployee ae
        INNER JOIN InternalUser iu
            ON iu.emailUser = ae.emailInternalUser
        WHERE 
            ae.nameEmployee LIKE @searchTerm
            OR ae.lastnames LIKE @searchTerm
            OR iu.emailUser LIKE @searchTerm;
    ";

            var dynamicUsers = _dbConnection.Query(sql, new
            {
                searchTerm = $"%{searchTerm}%"
            }).ToList<dynamic>();

            List<UserDTO> result = new List<UserDTO>();

            foreach (var user in dynamicUsers)
            {
                result.Add(MapToUserDTO(user));
            }

            return result;
        }

        public async Task<int> CreateInvitation(
            string email,
            string role,
            string token,
            DateTime expiryDate)
        {
    

            var newUserId = await _dbConnection.ExecuteScalarAsync<int>( 
                "INSERT INTO SystemUser DEFAULT VALUES; SELECT CAST(SCOPE_IDENTITY() AS TINYINT);"
                );

            var sql = @"


            INSERT INTO InternalUser (
                emailUser,
                userID,
                hashPasswordUser,
                isActive,
                invitationToken,
                invitationExpiryDate
            )
            VALUES (
                @Email,
                @UserID,
                '',
                0,
                @Token,
                @ExpiryDate
            );

            INSERT INTO AirlineEmployee (
                employeeID,
                emailInternalUser,
                nameEmployee,
                lastnames,
                isAdmin,
                isOperator
            )
            VALUES (
                @UserID,
                @Email,
                '',
                '',
                @IsAdmin,
                @IsOperator
            );
    ";

            await _dbConnection.ExecuteAsync(sql, new
            {
                UserID = newUserId,
                Email = email,
                Token = token,
                ExpiryDate = expiryDate,
                IsAdmin = role == "Admin" ? 1 : 0,
                IsOperator = role == "Operator" ? 1 : 0
            });

            return newUserId;
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            var sql = "SELECT COUNT(1) FROM InternalUser WHERE emailUser = @Email";
            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { Email = email });
        }

        public async Task<InvitationData?> GetInvitationByToken(string token)
        {
            var sql = @"
        SELECT 
            iu.emailUser AS Email,
            CASE
                WHEN ae.isAdmin = 1 THEN 'Admin'
                WHEN ae.isOperator = 1 THEN 'Operator'
                ELSE 'User'
            END AS Role,
                iu.invitationToken AS Token,
                iu.invitationExpiryDate AS ExpiryDate
            FROM InternalUser iu
            LEFT JOIN AirlineEmployee ae
                ON ae.emailInternalUser = iu.emailUser
            WHERE iu.invitationToken = @Token
                AND iu.isActive = 0;
        ";

            return await _dbConnection.QueryFirstOrDefaultAsync<InvitationData>(
                sql,
                new { Token = token }
            );
        }

        public async Task<bool> CompleteRegistration(
    string token,
    string FullName,
    string id,
    string passwordHash)
        {
            var names = FullName.Trim().Split(' ', 2);

            var firstName = names[0];

            var lastName = names.Length > 1
                ? names[1]
                : "";

            var sql = @"

        UPDATE InternalUser
        SET 
            hashPasswordUser = @PasswordHash,
            isActive = 1,
            invitationToken = NULL,
            invitationExpiryDate = NULL
        WHERE invitationToken = @Token
            AND isActive = 0;

        UPDATE AirlineEmployee
        SET
            nameEmployee = @FirstName,
            lastnames = @LastName
        WHERE emailInternalUser = (
            SELECT emailUser
            FROM InternalUser
            WHERE invitationToken = @Token
        );
    ";

            var rowsAffected = await _dbConnection.ExecuteAsync(sql, new
            {
                Token = token,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName
            });

            return rowsAffected > 0;
        }

        public async Task<UserModel?> GetUserByEmail(string email)
        {
            var sql = @"
        --Seleccione el ID de AirlineEmployee, junte el nombre y apellido y tratelo como Fullname y el Email.
        SELECT ae.employeeID AS Id,
        ae.nameEmployee + ' ' + ae.lastnames AS FullName,
        iu.emailUser AS Email,
        'Admin' AS Role,
        iu.hashPasswordUser AS PasswordHash,
        1 AS IsActive
            FROM InternalUser iu
            INNER JOIN AirlineEmployee ae
                ON ae.emailInternalUser = iu.emailUser
            WHERE iu.emailUser = @Email;
        ";

            return await _dbConnection.QueryFirstOrDefaultAsync<UserModel>(
                sql,
                new { Email = email }
            );
        }

        public async Task<UserModel?> GetUserById(int id)
        {
            var sql = @"
                SELECT 
                    ae.employeeID AS Id,
                    ae.nameEmployee + ' ' + ae.lastnames AS FullName,
                    iu.emailUser AS Email,
                    CASE
                        WHEN ae.isAdmin = 1 THEN 'Admin'
                        WHEN ae.isOperator = 1 THEN 'Operator'
                        ELSE 'User'
                    END AS Role,
                    iu.hashPasswordUser AS PasswordHash,
                    1 AS IsActive
                FROM InternalUser iu
                INNER JOIN AirlineEmployee ae
                    ON ae.emailInternalUser = iu.emailUser
                WHERE ae.employeeID = @Id;
            ";

            return await _dbConnection.QueryFirstOrDefaultAsync<UserModel>(
                sql,
                new { Id = id }
            );
        }
        public async Task<AirlineEmployee?> GetAirlineEmployeeByIdAsync(byte employeeId)
        {
            const string sql = @"
                SELECT employeeID, emailInternalUser, lastnames, nameEmployee, isAdmin, isOperator
                FROM AirlineEmployee
                WHERE employeeID = @employeeId";
            
            return await _dbConnection.QueryFirstOrDefaultAsync<AirlineEmployee>(sql, new { employeeId });
        }

        public async Task UpdateAirlineEmployeeAsync(byte employeeId, string? firstName, string? lastName, bool? isAdmin, bool? isOperator)
        {
            var sql = new StringBuilder("UPDATE AirlineEmployee SET ");
            var parameters = new DynamicParameters();
            parameters.Add("@employeeId", employeeId);
            
            if (firstName != null)
            {
                sql.Append("nameEmployee = @firstName, ");
                parameters.Add("@firstName", firstName);
            }
            
            if (lastName != null)
            {
                sql.Append("lastnames = @lastName, ");
                parameters.Add("@lastName", lastName);
            }
            
            if (isAdmin.HasValue)
            {
                sql.Append("isAdmin = @isAdmin, ");
                parameters.Add("@isAdmin", isAdmin.Value);
            }
            
            if (isOperator.HasValue)
            {
                sql.Append("isOperator = @isOperator, ");
                parameters.Add("@isOperator", isOperator.Value);
            }
            
            var finalSql = sql.ToString().TrimEnd(',', ' ');
            finalSql += " WHERE employeeID = @employeeId";
            
            await _dbConnection.ExecuteAsync(finalSql, parameters);
        }

        public async Task UpdateInternalUserActiveStatusAsync(byte employeeId, bool? isActive)
        {
            if (!isActive.HasValue) return;
            
            const string sql = @"
                UPDATE InternalUser 
                SET isActive = @isActive
                WHERE userID = (SELECT userID FROM AirlineEmployee WHERE employeeID = @employeeId)";
            
            await _dbConnection.ExecuteAsync(sql, new { employeeId, isActive = isActive.Value });
        }

        public async Task<byte> GetCurrentUserIdFromEmailAsync(string email)
        {
            const string sql = @"
                SELECT ae.employeeID
                FROM AirlineEmployee ae
                WHERE ae.emailInternalUser = @email";
            
            return await _dbConnection.QueryFirstOrDefaultAsync<byte>(sql, new { email });
        }

    }
}



