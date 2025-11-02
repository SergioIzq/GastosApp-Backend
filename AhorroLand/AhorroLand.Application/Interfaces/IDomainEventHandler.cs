using AhorroLand.Shared.Domain.Interfaces;

namespace AhorroLand.Application.Interfaces
{
    public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task Handle(TEvent notification, CancellationToken cancellationToken);
    }
}
