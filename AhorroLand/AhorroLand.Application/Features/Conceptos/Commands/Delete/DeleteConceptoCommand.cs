using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.Conceptos.Commands;

/// <summary>
/// Representa la solicitud para eliminar un Cliente por su identificador.
/// </summary>
// Hereda de AbsDeleteCommand<Entidad>
public sealed record DeleteConceptoCommand(Guid Id)
    : AbsDeleteCommand<Concepto>(Id);