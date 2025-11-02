using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Clientes.Events;

public sealed record ClienteCreatedDomainEvent(Guid Id) : DomainEventBase;
