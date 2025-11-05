using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Gastos.Queries;

/// <summary>
/// Representa la solicitud para crear un nuevo Gasto.
/// </summary>
// Hereda de AbsCreateCommand<Entidad, DTO de Respuesta>
public sealed record GetGastoByIdQuery(Guid Id) : AbsGetByIdQuery<Gasto, GastoDto>(Id)
{
}