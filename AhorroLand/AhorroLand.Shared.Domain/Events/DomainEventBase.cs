using AhorroLand.Shared.Domain.Interfaces;

namespace AhorroLand.Shared.Domain.Events;

public abstract record DomainEventBase : IDomainEvent
{
    public Guid EventId { get; init; }
    public DateTime OcurredOn { get; init; }

    protected DomainEventBase()
    {
        EventId = Guid.NewGuid();
        OcurredOn = DateTime.UtcNow;
    }
}