using System.Threading.Tasks;

namespace AirDreams.API.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmail(string email, string token, string tipoUsuario);
        Task SendWelcomeEmail(string email, string nombreCompleto);
    }
}