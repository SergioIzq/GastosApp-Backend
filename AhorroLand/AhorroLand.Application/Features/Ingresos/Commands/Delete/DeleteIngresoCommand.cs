using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.Ingresos.Commands;

/// <summary>
/// Representa la solicitud para eliminar una Ingreso por su identificador.
/// </summary>
// Hereda de AbsDeleteCommand<Entidad>
public sealed record DeleteIngresoCommand(Guid Id)
    : AbsDeleteCommand<Ingreso>(Id);