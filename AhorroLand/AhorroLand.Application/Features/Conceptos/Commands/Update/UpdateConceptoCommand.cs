using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Conceptos.Commands;

/// <summary>
/// Representa la solicitud para actualizar un nuevo concepto.
/// </summary>
// Hereda de AbsUpadteCommand<Entidad, DTO de Respuesta>
public sealed record UpdateConceptoCommand : AbsUpdateCommand<Concepto, ConceptoDto>
{
    /// <summary>
    /// Nombre del nuevo concepto.
    /// </summary>
    public required string Nombre { get; init; }

    public required Guid CategoriaId { get; init; }
}