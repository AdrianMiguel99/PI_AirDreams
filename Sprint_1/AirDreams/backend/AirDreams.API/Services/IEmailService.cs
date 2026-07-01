using AirDreams.API.DTOs;

namespace AirDreams.API.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmail(
            string toEmail,
            string token,
            string role
        );

        Task SendWelcomeEmail(
            string toEmail,
            string fullName
        );

        Task SendPurchaseConfirmationEmail(
            string toEmail,
            ConfirmPurchaseDTO purchase,
            byte[] invoicePdf,
            byte[] itineraryPdf
        );

        Task SendCancellationEmailAsync(
            string toEmail,
            string token
        );
    }
}