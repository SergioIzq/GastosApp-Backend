using AhorroLand.Domain.Traspasos;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Traspasos.Commands;

public sealed record CreateTraspasoCommand : AbsCreateCommand<Traspaso, TraspasoDto>
{
 public required Guid CuentaOrigenId { get; init; }
 public required Guid CuentaDestinoId { get; init; }
 public required decimal Importe { get; init; }
 public required DateTime Fecha { get; init; }
 public string? Descripcion { get; init; }
}
