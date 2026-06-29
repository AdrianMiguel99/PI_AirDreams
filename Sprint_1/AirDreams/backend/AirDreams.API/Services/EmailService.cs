using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using AirDreams.API.Models;
using AirDreams.API.Models.Dtos;

namespace AirDreams.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendInvitationEmail(
            string toEmail,
            string token,
            string role)
        {
            var frontendUrl = "http://localhost:5173";

            var registerLink =
                $"{frontendUrl}/completar-registro?token={token}";

            var subject = "Invitación a AirDreams";

            var body = $@"
                <h1>¡Bienvenido a AirDreams!</h1>

                <p>
                    Has sido invitado como
                    <strong>{role}</strong>.
                </p>

                <a href='{registerLink}'>
                    Completar registro
                </a>
            ";

            await SendEmailAsync(
                toEmail,
                subject,
                body
            );
        }

        public async Task SendWelcomeEmail(
            string toEmail,
            string fullName)
        {
            var subject =
                "Bienvenido a AirDreams";

            var body = $@"
                <h1>
                    ¡Bienvenido {fullName}!
                </h1>

                <p>
                    Tu registro fue completado.
                </p>
            ";

            await SendEmailAsync(
                toEmail,
                subject,
                body
            );
        }

        public async Task SendPurchaseConfirmationEmail(
            string toEmail,
            ConfirmPurchaseDto purchase,
            byte[] invoicePdf,
            byte[] itineraryPdf
        )
        {
            var subject =
                $"Confirmación de compra - {purchase.TransactionId}";

            var body = $@"
                <h1>
                    ¡Compra confirmada!
                </h1>

                <p>
                    Hola {purchase.BuyerName},
                </p>

                <p>
                    Gracias por comprar con AirDreams.
                </p>

                <p>
                    Adjuntamos:
                </p>

                <ul>
                    <li>Factura (PDF)</li>
                    <li>Itinerario (PDF)</li>
                </ul>

                <p>
                    Transacción:
                    <strong>
                        {purchase.TransactionId}
                    </strong>
                </p>

                <p>
                    ¡Buen viaje!
                </p>
            ";

            await SendEmailWithAttachmentsAsync(
                toEmail,
                subject,
                body,
                invoicePdf,
                itineraryPdf
            );
        }
        public async Task SendCancellationEmailAsync(
            string toEmail,
            string token)
        {
            var frontendUrl = "http://localhost:5173";

            var cancellationLink =
                $"{frontendUrl}/cancel-reservation?token={token}";

            var subject =
                "Confirmación de cancelación de reserva - AirDreams";

            var body = $@"
                <h1>Solicitud de cancelación de reserva</h1>

                <p>
                    Se ha solicitado la cancelación de su reserva en AirDreams.
                </p>

                <p>
                    Para confirmar la cancelación, haga clic en el siguiente enlace:
                </p>

                <p>
                    <a href='{cancellationLink}'>
                        Confirmar cancelación
                    </a>
                </p>

                <p>
                    <strong>Importante:</strong>
                    esta acción es irreversible.
                </p>

                <p>
                    No se realizará devolución de dinero.
                </p>

                <p>
                    Si usted no realizó esta solicitud, ignore este correo.
                </p>
            ";

            await SendEmailAsync(
                toEmail,
                subject,
                body
            );
        }

        private async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            if (
                string.IsNullOrWhiteSpace(
                    _emailSettings.Username
                )
                ||
                string.IsNullOrWhiteSpace(
                    _emailSettings.Password
                )
            )
            {
                _logger.LogInformation(
                    $"[EMAIL SIMULADO] {toEmail}"
                );

                return;
            }

            try
            {
                var message =
                    new MimeMessage();

                message.From.Add(
                    new MailboxAddress(
                        _emailSettings.FromName,
                        _emailSettings.FromEmail
                    )
                );

                message.To.Add(
                    new MailboxAddress(
                        "",
                        toEmail
                    )
                );

                message.Subject =
                    subject;

                var builder =
                    new BodyBuilder
                    {
                        HtmlBody = body
                    };

                message.Body =
                    builder.ToMessageBody();

                using var client =
                    new SmtpClient();

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

                await client.SendAsync(
                    message
                );

                await client.DisconnectAsync(
                    true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error enviando correo"
                );

                throw;
            }
        }

        private async Task SendEmailWithAttachmentsAsync(
            string toEmail,
            string subject,
            string body,
            byte[] invoicePdf,
            byte[] itineraryPdf
        )
        {
            try
            {
                var message =
                    new MimeMessage();

                message.From.Add(
                    new MailboxAddress(
                        _emailSettings.FromName,
                        _emailSettings.FromEmail
                    )
                );

                message.To.Add(
                    new MailboxAddress(
                        "",
                        toEmail
                    )
                );

                message.Subject =
                    subject;

                var builder =
                    new BodyBuilder
                    {
                        HtmlBody = body
                    };

                builder.Attachments.Add(
                    "Factura-AirDreams.pdf",
                    invoicePdf
                );

                builder.Attachments.Add(
                    "Itinerario-AirDreams.pdf",
                    itineraryPdf
                );

                message.Body =
                    builder.ToMessageBody();

                using var client =
                    new SmtpClient();

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

                await client.SendAsync(
                    message
                );

                await client.DisconnectAsync(
                    true
                );

                _logger.LogInformation(
                    $"Correo enviado con adjuntos a {toEmail}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error enviando correo con PDFs"
                );

                throw;
            }
        }
    }
}