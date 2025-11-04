using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Cuentas.Commands;

/// <summary>
/// Representa la solicitud para crear un nuevo Cuenta.
/// </summary>
// Hereda de AbsCreateCommand<Entidad, DTO de Respuesta>
public sealed record GetCuentaByIdQuery(Guid Id) : AbsGetByIdQuery<Cuenta, CuentaDto>(Id)
{
}