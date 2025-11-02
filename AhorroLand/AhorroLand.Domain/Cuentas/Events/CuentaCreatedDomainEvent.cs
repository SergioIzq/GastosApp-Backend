using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Cuentas.Events;

public sealed record CuentaCreatedDomainEvent(Guid Id) : DomainEventBase;
