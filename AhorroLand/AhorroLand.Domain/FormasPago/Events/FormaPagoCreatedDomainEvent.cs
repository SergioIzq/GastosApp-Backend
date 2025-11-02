using AhorroLand.Shared.Domain.Events;

namespace AhorroLand.Domain.FormasPago.Events;

public sealed record FormaPagoCreatedDomainEvent(Guid Id) : DomainEventBase;
