using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Ingresos.Commands;

public sealed record CreateIngresoCommand : AbsCreateCommand<Ingreso, IngresoDto>
{
    public required decimal Importe { get; init; }
    public required DateTime Fecha { get; init; }
    public string? Descripcion { get; init; }

    public required Guid CategoriaId { get; init; }
    public required string CategoriaNombre { get; init; }

    public required Guid ConceptoId { get; init; }
    public required string ConceptoNombre { get; init; }

    public required Guid ClienteId { get; init; }
    public required string ClienteNombre { get; init; }

    public required Guid PersonaId { get; init; }
    public required string PersonaNombre { get; init; }

    public required Guid CuentaId { get; init; }
    public required string CuentaNombre { get; init; }

    public required Guid FormaPagoId { get; init; }
    public required string FormaPagoNombre { get; init; }

    public required Guid UsuarioId { get; init; }
}
