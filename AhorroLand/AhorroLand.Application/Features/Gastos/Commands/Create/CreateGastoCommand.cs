using AhorroLand.Domain.Gastos;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Gastos.Commands;

public sealed record CreateGastoCommand : AbsCreateCommand<Gasto, GastoDto>
{
    public required decimal Importe { get; init; }
    public required DateTime Fecha { get; init; }
    public string? Descripcion { get; init; }

    public required Guid CategoriaId { get; init; }
    public required Guid ConceptoId { get; init; }
    public required Guid ProveedorId { get; init; }
    public required Guid PersonaId { get; init; }
    public required Guid CuentaId { get; init; }
    public required Guid FormaPagoId { get; init; }
    public required Guid UsuarioId { get; init; }
}
