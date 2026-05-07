namespace AirDreams.API.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmail(string toEmail, string token, string role);
        
        Task SendWelcomeEmail(string toEmail, string fullName);
    }
}