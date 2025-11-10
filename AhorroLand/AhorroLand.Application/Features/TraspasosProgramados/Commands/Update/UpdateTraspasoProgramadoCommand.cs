using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.TraspasosProgramados.Commands;

public sealed record UpdateTraspasoProgramadoCommand : AbsUpdateCommand<TraspasoProgramado, TraspasoProgramadoDto>
{
    public required Guid CuentaOrigenId { get; init; }
    public required Guid CuentaDestinoId { get; init; }
    public required decimal Importe { get; init; }
    public required DateTime FechaEjecucion { get; init; }
    public required string Frecuencia { get; init; }
    public required Guid UsuarioId { get; init; }
    public required string HangfireJobId { get; init; }
    public string? Descripcion { get; init; }
}
