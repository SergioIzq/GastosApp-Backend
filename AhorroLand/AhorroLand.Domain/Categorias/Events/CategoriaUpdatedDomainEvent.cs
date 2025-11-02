using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Categorias.Events;

public sealed record CategoriaUpdatedDomainEvent(Guid Id) : DomainEventBase;
