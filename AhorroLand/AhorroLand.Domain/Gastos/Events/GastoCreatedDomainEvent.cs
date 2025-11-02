using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Gastos.Events;

public sealed record GastoCreatedDomainEvent(Guid Id) : DomainEventBase;
