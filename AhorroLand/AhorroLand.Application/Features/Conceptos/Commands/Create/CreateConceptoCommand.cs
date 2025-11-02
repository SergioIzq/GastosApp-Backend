using AhorroLand.Domain.Conceptos;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Conceptos.Commands;

/// <summary>
/// Representa la solicitud para crear un nuevo Concepto.
/// </summary>
public sealed record CreateConceptoCommand : AbsCreateCommand<Concepto, ConceptoDto>
{
    public required string Nombre { get; init; }
    public required Guid CategoriaId { get; init; }
    public required Guid UsuarioId { get; init; }
}
