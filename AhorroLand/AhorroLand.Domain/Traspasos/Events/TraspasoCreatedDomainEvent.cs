using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Traspasos.Events;

public sealed record TraspasoCreatedDomainEvent(Guid id) : DomainEventBase;