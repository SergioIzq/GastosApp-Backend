using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Personas.Queries;

/// <summary>
/// Representa la consulta para obtener una lista paginada de Personas.
/// </summary>
// Hereda de AbsGetPagedListQuery<Entidad, DTO de Ítem>
public sealed record GetPersonasPagedListQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string? SortColumn = null,
    string? SortOrder = null
) : AbsGetPagedListQuery<Persona, PersonaDto>(Page, PageSize);