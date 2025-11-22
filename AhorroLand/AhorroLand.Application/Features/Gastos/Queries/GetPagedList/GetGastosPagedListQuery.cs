using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Gastos.Queries;

/// <summary>
/// Representa la consulta para obtener una lista paginada de Gastos.
/// </summary>
// Hereda de AbsGetPagedListQuery<Entidad, DTO de Ítem>
public sealed record GetGastosPagedListQuery : AbsGetPagedListQuery<Gasto, GastoDto>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortColumn { get; init; }
    public string? SortOrder { get; init; }

    // 🔥 CRÍTICO: Permite asignar el UsuarioId después de la creación
    public Guid? UsuarioId { get; set; }

    public GetGastosPagedListQuery(int page, int pageSize, string? searchTerm = null, string? sortColumn = null, string? sortOrder = null)
        : base(page, pageSize, null) // Null aquí porque lo asignaremos después
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        SortColumn = sortColumn;
        SortOrder = sortOrder;
    }
}