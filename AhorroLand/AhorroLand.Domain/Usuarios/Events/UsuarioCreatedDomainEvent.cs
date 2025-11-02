using AhorroLand.Shared.Domain.Events;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain.Usuarios.Events;

public sealed record UsuarioCreatedDomainEvent(Guid Id, Email Correo) : DomainEventBase;