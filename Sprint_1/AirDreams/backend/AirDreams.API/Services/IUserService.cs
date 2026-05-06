using AirDreams.API.Models;
using AirDreams.API.DTOs;
using System.Threading.Tasks;

namespace AirDreams.API.Services
{
    public interface IUserService
    {
        Task<(bool success, string message)> SendInvitation(InvitationModel model);
        
        Task<(bool success, string message)> CompleteRegistration(CompleteRegistrationModel model);
        
        Task<(bool success, string message, UserModel? user)> Login(LoginModel model);
        
        Task<InvitationValidationResult?> ValidateInvitationToken(string token);

        List<UserDTO> GetAll();

        List<UserDTO> Search(string searchTerm);
    }
    
    public class InvitationValidationResult
    {
        public bool IsValid { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
    }
}
