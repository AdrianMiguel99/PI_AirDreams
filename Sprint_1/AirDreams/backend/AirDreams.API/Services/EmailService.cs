using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AirDreams.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendInvitationEmail(string email, string token, string tipoUsuario)
        {
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:5173";
            var link = $"{baseUrl}/completar-registro?token={token}";

            var subject = "Bienvenido al sistema de Air Dreams";
            var body = $@"
                <h2>Bienvenido al sistema de Air Dreams</h2>
                <p>Has sido invitado como <strong>{tipoUsuario}</strong> a la plataforma.</p>
                <p>Utilice el siguiente link para completar su registro:</p>
                <p><a href='{link}'>{link}</a></p>
                <p>Este link expirará en 48 horas.</p>
                <br/>
                <p>Saludos,<br/>Equipo de Air Dreams</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmail(string email, string nombreCompleto)
        {
            var subject = "¡Bienvenido a Air Dreams!";
            var body = $@"
                <h2>¡Bienvenido {nombreCompleto}!</h2>
                <p>Tu registro ha sido completado exitosamente.</p>
                <p>Ya puedes iniciar sesión en la plataforma con tu correo y la contraseña que estableciste.</p>
                <br/>
                <p>Saludos,<br/>Equipo de Air Dreams</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"] ?? username;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                // Si no hay configuración SMTP, solo logueamos (modo desarrollo)
                _logger.LogInformation($"[EMAIL SIMULADO] Para: {to} | Asunto: {subject} | Body: {body}");
                return;
            }

            try
            {
                using var client = new SmtpClient(host, port);
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(username, password);

                var message = new MailMessage(fromEmail, to, subject, body);
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email enviado exitosamente a {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar email a {to}");
                throw;
            }
        }
    }
}