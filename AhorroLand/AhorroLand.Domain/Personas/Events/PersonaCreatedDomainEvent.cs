using AhorroLandBackend.Domain.Abstractions;

namespace AhorroLandBackend.Domain.Personas.Events;

public sealed record PersonaCreatedDomainEvent(Guid UserId) : IDomainEvent;
