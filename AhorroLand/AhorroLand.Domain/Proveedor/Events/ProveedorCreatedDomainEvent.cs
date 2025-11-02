using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Proveedores.Events;

public sealed record ProveedorCreatedDomainEvent(Guid id) : DomainEventBase;