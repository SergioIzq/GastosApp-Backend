using AhorroLand.Domain.Clientes;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.Categorias.Commands;

/// <summary>
/// Representa la solicitud para eliminar un Cliente por su identificador.
/// </summary>
// Hereda de AbsDeleteCommand<Entidad>
public sealed record DeleteClienteCommand(Guid Id)
    : AbsDeleteCommand<Cliente>(Id);