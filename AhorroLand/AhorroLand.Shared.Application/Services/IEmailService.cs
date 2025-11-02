using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Shared.Application.Abstractions.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Encola un mensaje para ser enviado en segundo plano.
        /// Esta operación es rápida y no bloquea el hilo de la API.
        /// </summary>
        void EnqueueEmail(EmailMessage message);
    }
}