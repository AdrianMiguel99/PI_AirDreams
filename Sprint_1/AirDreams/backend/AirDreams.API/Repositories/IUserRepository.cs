using AirDreams.API.Models;
using AirDreams.API.DTOs;
using System.Threading.Tasks;

namespace AirDreams.API.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateInvitation(string email, string tipoUsuario, string token, DateTime expiryDate);
        
        Task<bool> ExistsByEmail(string email);
        
        Task<InvitationData?> GetInvitationByToken(string token);
        
        Task<bool> CompleteRegistration(string token, string nombreCompleto, string cedula, string passwordHash);
        
        Task<UserModel?> GetUserByEmail(string email);
        
        Task<UserModel?> GetUserById(int id);

        List<UserDTO> GetAll();
        List<UserDTO> Search(string searchTerm);

        Task<AirlineEmployee?> GetAirlineEmployeeByIdAsync(byte employeeId);
        Task UpdateAirlineEmployeeAsync(byte employeeId, string? firstName, string? lastName, bool? isAdmin, bool? isOperator);
        Task UpdateInternalUserActiveStatusAsync(byte employeeId, bool? isActive);
        Task<byte> GetCurrentUserIdFromEmailAsync(string email);
    }
    
    public class InvitationData
    {
        public string Email { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
        
        public DateTime ExpiryDate { get; set; }
    }
}
