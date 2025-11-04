using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

namespace AhorroLand.Application.Features.Gastos.Commands;

public sealed record CreateGastoCommand : AbsCreateCommand<Gasto, Guid>
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
