namespace AhorroLand.Shared.Application.Interfaces;

/// <summary>
/// Interfaz para el envío de emails.
/// Abstrae la lógica de envío de correos electrónicos.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envía un correo electrónico.
    /// </summary>
    /// <param name="toEmail">Dirección de correo del destinatario.</param>
    /// <param name="subject">Asunto del correo.</param>
    /// <param name="body">Cuerpo del correo (puede ser HTML).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}
