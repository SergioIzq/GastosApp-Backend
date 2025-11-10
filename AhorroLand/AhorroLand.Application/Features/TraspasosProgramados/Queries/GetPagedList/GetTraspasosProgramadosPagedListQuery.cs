using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.TraspasosProgramados.Queries;

public sealed record GetTraspasosProgramadosPagedListQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string? SortColumn = null,
    string? SortOrder = null
) : AbsGetPagedListQuery<TraspasoProgramado, TraspasoProgramadoDto>(Page, PageSize);
