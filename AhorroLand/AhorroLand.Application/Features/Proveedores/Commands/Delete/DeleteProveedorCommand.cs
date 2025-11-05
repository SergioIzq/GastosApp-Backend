using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.Proveedores.Commands;

/// <summary>
/// Representa la solicitud para eliminar una Proveedor por su identificador.
/// </summary>
// Hereda de AbsDeleteCommand<Entidad>
public sealed record DeleteProveedorCommand(Guid Id)
    : AbsDeleteCommand<Proveedor>(Id);