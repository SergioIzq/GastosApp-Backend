using AhorroLand.Infrastructure.Configuration.Settings;
using AhorroLand.Shared.Application.Dtos;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AhorroLand.Infrastructure.Services;

/// <summary>
/// Servicio en segundo plano que procesa la cola de emails.
/// Utiliza MailKit para envío asíncrono y eficiente de correos electrónicos.
/// </summary>
public class EmailBackgroundSender : BackgroundService
{
    private readonly QueuedEmailService _emailQueue;
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailBackgroundSender> _logger;

    public EmailBackgroundSender(
    QueuedEmailService emailQueue,
IOptions<EmailSettings> options,
        ILogger<EmailBackgroundSender> logger)
    {
        _emailQueue = emailQueue;
        _settings = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📧 Servicio de envío de emails en segundo plano iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_emailQueue.TryDequeue(out var message))
            {
                await SendEmailInternalAsync(message!, stoppingToken);
            }
            else
            {
                // Esperamos un momento si la cola está vacía
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("📧 Servicio de envío de emails en segundo plano detenido");
    }

    private async Task SendEmailInternalAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            // 1. Crear el mensaje MIME
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_settings.FromName ?? "AhorroLand", _settings.SmtpUser));
            mimeMessage.To.Add(MailboxAddress.Parse(message.To));
            mimeMessage.Subject = message.Subject;

            var builder = new BodyBuilder { HtmlBody = message.Body };
            mimeMessage.Body = builder.ToMessageBody();

            // 2. Conexión y envío con MailKit (más eficiente y moderno)
            await client.ConnectAsync(
      _settings.SmtpServer,
          _settings.SmtpPort,
         _settings.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls,
                cancellationToken);

            // Autenticación si está configurada
            if (!string.IsNullOrEmpty(_settings.SmtpUser) && !string.IsNullOrEmpty(_settings.SmtpPass))
            {
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass, cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);

            _logger.LogInformation("✅ Email enviado exitosamente a {To}", message.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al enviar email a {To}: {Message}", message.To, ex.Message);

            // TODO: Implementar lógica de reintento o dead-letter queue
            // Por ahora, el email se pierde si falla
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}
