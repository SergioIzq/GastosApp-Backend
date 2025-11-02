using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Ingresos.Events;

public sealed record IngresoCreatedDomainEvent(Guid Id) : DomainEventBase;
