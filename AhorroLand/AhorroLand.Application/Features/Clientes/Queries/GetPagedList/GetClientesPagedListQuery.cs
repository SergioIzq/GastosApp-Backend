using AhorroLand.Domain.Categorias;
using AhorroLand.Domain.Clientes;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Categorias.Queries;

/// <summary>
/// Representa la consulta para obtener una lista paginada de Categorías.
/// </summary>
// Hereda de AbsGetPagedListQuery<Entidad, DTO de Ítem>
public sealed record GetClientesPagedListQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string? SortColumn = null,
    string? SortOrder = null
) : AbsGetPagedListQuery<Cliente, ClienteDto>(Page, PageSize);