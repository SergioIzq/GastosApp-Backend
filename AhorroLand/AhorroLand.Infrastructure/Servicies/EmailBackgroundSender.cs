using AhorroLand.Infrastructure.Configuration.Settings;
using AhorroLand.Shared.Application.Dtos;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AhorroLand.Infrastructure.Servicies
{

    // AhorroLand.Infrastructure.Services
    public class EmailBackgroundSender : BackgroundService
    {
        private readonly QueuedEmailService _emailQueue;
        private readonly EmailSettings _settings;

        // Inyectamos la cola y la configuración
        public EmailBackgroundSender(QueuedEmailService emailQueue, IOptions<EmailSettings> options)
        {
            _emailQueue = emailQueue;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // El bucle principal del background service
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_emailQueue.TryDequeue(out var message))
                {
                    await SendEmailInternalAsync(message!);
                }
                else
                {
                    // Esperamos un momento si la cola está vacía
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        // Mantiene la lógica original de MailKit, pero dentro del background thread
        private async Task SendEmailInternalAsync(EmailMessage message)
        {
            using var client = new SmtpClient();

            try
            {
                // 1. Crear el mensaje (Rápido)
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("AhorroLand", _settings.SmtpUser));
                mimeMessage.To.Add(MailboxAddress.Parse(message.To));
                mimeMessage.Subject = message.Subject;
                var builder = new BodyBuilder { HtmlBody = message.Body };
                mimeMessage.Body = builder.ToMessageBody();

                // 2. Conexión/Envío (Lento, pero en un hilo no bloqueante)
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
                await client.SendAsync(mimeMessage);
            }
            catch (Exception ex)
            {
                // ¡IMPORTANTE! Aquí manejarías errores, logging, o re-enqueuar el email para reintento.
                Console.WriteLine($"Error al enviar email a {message.To}: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
