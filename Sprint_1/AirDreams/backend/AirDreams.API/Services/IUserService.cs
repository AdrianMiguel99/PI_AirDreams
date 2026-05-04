using AirDreams.API.Models;
using System.Threading.Tasks;

namespace AirDreams.API.Services
{
    public interface IUserService
    {
        // Administrador envía invitación
        Task<(bool success, string message)> SendInvitation(InvitationModel model);
        
        // Usuario completa registro
        Task<(bool success, string message)> CompleteRegistration(CompleteRegistrationModel model);
        
        // Login
        Task<(bool success, string message, UserModel? user)> Login(LoginModel model);
        
        // Validar token de invitación
        Task<InvitationValidationResult?> ValidateInvitationToken(string token);
    }
    
    public class InvitationValidationResult
    {
        public bool IsValid { get; set; }

        public string Email { get; set; } = string.Empty;

        public string TipoUsuario { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
    }
}
