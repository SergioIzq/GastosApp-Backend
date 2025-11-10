using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.IngresosProgramados.Queries;

public sealed record GetIngresosProgramadosPagedListQuery(
    int Page,
  int PageSize,
 string? SearchTerm = null,
    string? SortColumn = null,
    string? SortOrder = null
) : AbsGetPagedListQuery<IngresoProgramado, IngresoProgramadoDto>(Page, PageSize);
