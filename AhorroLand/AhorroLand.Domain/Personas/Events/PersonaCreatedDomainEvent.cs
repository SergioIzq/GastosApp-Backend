using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Personas.Events;

public sealed record PersonaCreatedDomainEvent(Guid id) : DomainEventBase;
