using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Conceptos.Commands;

/// <summary>
/// Representa la solicitud para crear un nuevo Concepto.
/// </summary>
// Hereda de AbsCreateCommand<Entidad, DTO de Respuesta>
public sealed record GetConceptoByIdQuery(Guid Id) : AbsGetByIdQuery<Concepto, ConceptoDto>(Id)
{
}