using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Conceptos.Events;

public sealed record ConceptoCreatedDomainEvent(Guid Id) : DomainEventBase;
