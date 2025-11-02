using AhorroLand.Domain.Cuentas;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Cuentas.Commands;

public sealed record CreateCuentaCommand : AbsCreateCommand<Cuenta, CuentaDto>
{
 public required string Nombre { get; init; }
 public required decimal Saldo { get; init; }
}
