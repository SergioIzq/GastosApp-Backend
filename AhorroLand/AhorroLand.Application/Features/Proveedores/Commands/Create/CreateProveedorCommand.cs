using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Proveedores.Commands;

public sealed record CreateProveedorCommand : AbsCreateCommand<Proveedor, ProveedorDto>
{
    public required string Nombre { get; init; }
    public required Guid UsuarioId { get; init; }
}
