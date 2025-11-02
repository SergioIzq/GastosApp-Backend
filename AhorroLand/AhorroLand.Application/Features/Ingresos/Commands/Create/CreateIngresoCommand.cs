using AhorroLand.Domain.Ingresos;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Ingresos.Commands;

public sealed record CreateIngresoCommand : AbsCreateCommand<Ingreso, IngresoDto>
{
 public required decimal Importe { get; init; }
 public required DateTime Fecha { get; init; }
 public string? Descripcion { get; init; }
 public required Guid ConceptoId { get; init; }
 public Guid? ProveedorId { get; init; }
 public Guid? PersonaId { get; init; }
 public required Guid CuentaId { get; init; }
 public required Guid FormaPagoId { get; init; }
}
