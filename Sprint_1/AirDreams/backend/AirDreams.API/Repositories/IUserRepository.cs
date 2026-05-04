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
        
        // Login
        Task<UserModel?> GetUserByEmail(string email);
        
        Task<UserModel?> GetUserById(int id);

        List<UserDTO> GetAll();
        List<UserDTO> Search(string searchTerm);
    }
    
    public class InvitationData
    {
        public string Email { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
        
        public DateTime ExpiryDate { get; set; }
    }
}
