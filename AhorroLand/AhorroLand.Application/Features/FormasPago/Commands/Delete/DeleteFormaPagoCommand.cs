using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.FormasPago.Commands;

/// <summary>
/// Representa la solicitud para eliminar una Cuenta por su identificador.
/// </summary>
// Hereda de AbsDeleteCommand<Entidad>
public sealed record DeleteFormaPagoCommand(Guid Id)
    : AbsDeleteCommand<FormaPago>(Id);