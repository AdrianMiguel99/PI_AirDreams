using System.Threading.Tasks;

namespace AirDreams.API.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmail(string email, string token, string Role);
        Task SendWelcomeEmail(string email, string FullName);
    }
}