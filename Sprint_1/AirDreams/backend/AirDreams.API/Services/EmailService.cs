using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using AirDreams.API.Models;

namespace AirDreams.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendInvitationEmail(string toEmail, string token, string role)
        {
            var frontendUrl = "http://localhost:5173";
            var registerLink = $"{frontendUrl}/completar-registro?token={token}";

            var subject = "Invitación a AirDreams";
            var body = $@"
                <h1>¡Bienvenido a AirDreams!</h1>
                <p>Has sido invitado como <strong>{role}</strong>.</p>
                <p>Haz clic en el siguiente enlace para completar tu registro:</p>
                <a href='{registerLink}' style='background-color:#4CAF50;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;'>
                    Completar registro
                </a>
                <p>El enlace expirará en 48 horas.</p>
                <p>Si no solicitaste esta invitación, ignora este correo.</p>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmail(string toEmail, string fullName)
        {
            var subject = "Bienvenido a AirDreams";
            var body = $@"
                <h1>¡Bienvenido {fullName}!</h1>
                <p>Tu registro se ha completado exitosamente.</p>
                <p>Ya puedes iniciar sesión en nuestra plataforma con tu correo y contraseña.</p>
                <a href='http://localhost:5173/login' style='background-color:#4CAF50;color:white;padding:10px 20px;text-decoration:none;border-radius:5px;'>
                    Iniciar sesión
                </a>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body,
                    TextBody = "Versión en texto plano del mensaje"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.Port,
                    _emailSettings.EnableSsl
                        ? SecureSocketOptions.SslOnConnect
                        : SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _emailSettings.Username,
                    _emailSettings.Password
                );

                await client.SendAsync(message);

                await client.DisconnectAsync(true);

                _logger.LogInformation($"Correo enviado exitosamente a {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo a {toEmail}");
                throw;
            }
        }
    }
}