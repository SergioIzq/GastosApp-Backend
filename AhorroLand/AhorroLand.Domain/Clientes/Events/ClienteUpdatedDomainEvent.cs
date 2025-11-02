using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Clientes.Events;

public sealed record ClienteUpdatedDomainEvent(Guid Id) : DomainEventBase;
