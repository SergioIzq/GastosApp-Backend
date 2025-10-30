using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.Categorias.Events;

public sealed record CategoriaCreatedDomainEvent(Guid Id) : DomainEventBase;
