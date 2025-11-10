using AhorroLand.Infrastructure.Configuration.Settings;
using AhorroLand.Shared.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AhorroLand.Infrastructure.Services.Auth;

/// <summary>
/// Implementación del servicio de envío de emails.
/// </summary>
public sealed class EmailSenderService : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSenderService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
        }
        catch (SmtpException ex)
        {
            throw new InvalidOperationException($"Error al enviar el correo electrónico: {ex.Message}", ex);
        }
    }
}
