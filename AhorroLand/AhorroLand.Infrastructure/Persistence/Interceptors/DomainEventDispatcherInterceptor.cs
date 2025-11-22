using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AhorroLand.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor de EF Core que publica los eventos de dominio después de que la transacción se confirma exitosamente.
/// </summary>
public sealed class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;
    private static readonly int MaxDegreeOfParallelism = Environment.ProcessorCount;
    private static readonly int BatchSize = 32;

    public DomainEventDispatcherInterceptor(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await PublishDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublishDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var entitiesWithEvents = new List<(AbsEntity entity, List<IDomainEvent> events)>();

        foreach (var entry in context.ChangeTracker.Entries<AbsEntity>())
        {
            var entity = entry.Entity;
            if (entity.DomainEvents.Count > 0)
            {
                // Copiar eventos a una lista local para preservarlos en caso de fallo
                var eventsCopy = new List<IDomainEvent>(entity.DomainEvents);
                entitiesWithEvents.Add((entity, eventsCopy));
            }
        }

        if (entitiesWithEvents.Count == 0)
            return;

        using var semaphore = new SemaphoreSlim(MaxDegreeOfParallelism);
        var publishTasks = new List<Task>(entitiesWithEvents.Count);

        foreach (var (entity, events) in entitiesWithEvents)
        {
            foreach (var domainEvent in events)
            {
                await semaphore.WaitAsync(cancellationToken);

                var publishTask = PublishEventWithSemaphoreAsync(domainEvent, semaphore, cancellationToken);
                publishTasks.Add(publishTask);

                if (publishTasks.Count >= BatchSize)
                {
                    await Task.WhenAll(publishTasks);
                    publishTasks.Clear();
                }
            }
        }

        // Esperar los eventos restantes
        if (publishTasks.Count > 0)
        {
            await Task.WhenAll(publishTasks);
        }

        foreach (var (entity, _) in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
        }
    }

    private async Task PublishEventWithSemaphoreAsync(
        IDomainEvent domainEvent,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        try
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
