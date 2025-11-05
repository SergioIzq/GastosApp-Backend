using AhorroLand.Shared.Application.Abstractions.Services;
using AhorroLand.Shared.Application.Dtos;
using System.Collections.Concurrent;

namespace AhorroLand.Infrastructure.Servicies
{
    public class QueuedEmailService : IEmailService
    {
        // La cola concurrente es segura para hilos
        private readonly ConcurrentQueue<EmailMessage> _emailQueue = new();

        public void EnqueueEmail(EmailMessage message)
        {
            _emailQueue.Enqueue(message);
        }

        // Método para ser usado por el Background Worker
        public bool TryDequeue(out EmailMessage? message)
        {
            return _emailQueue.TryDequeue(out message);
        }
    }
}
