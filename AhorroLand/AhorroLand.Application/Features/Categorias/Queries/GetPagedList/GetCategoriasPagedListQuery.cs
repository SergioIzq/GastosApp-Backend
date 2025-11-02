using AhorroLand.Domain.Categorias;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Categorias.Queries;

/// <summary>
/// Representa la consulta para obtener una lista paginada de Categorías.
/// </summary>
// Hereda de AbsGetPagedListQuery<Entidad, DTO de Ítem>
public sealed record GetCategoriasPagedListQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string? SortColumn = null,
    string? SortOrder = null
) : AbsGetPagedListQuery<Categoria, CategoriaDto>(Page, PageSize);