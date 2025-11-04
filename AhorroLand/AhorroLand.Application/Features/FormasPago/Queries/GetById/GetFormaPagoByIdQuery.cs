using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.FormasPago.Commands;

/// <summary>
/// Representa la solicitud para crear un nuevo FormaPago.
/// </summary>
// Hereda de AbsCreateCommand<Entidad, DTO de Respuesta>
public sealed record GetFormaPagoByIdQuery(Guid Id) : AbsGetByIdQuery<FormaPago, FormaPagoDto>(Id)
{
}