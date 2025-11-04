using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.Categorias.Commands;

/// <summary>
/// Representa la solicitud para eliminar una Categoría por su identificador.
/// </summary>
// Hereda de AbsDeleteCommand<Entidad>
public sealed record DeleteCategoriaCommand(Guid Id)
    : AbsDeleteCommand<Categoria>(Id);