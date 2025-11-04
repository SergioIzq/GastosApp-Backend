using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AhorroLand.Application.Features.Conceptos.Queries;

/// <summary>
/// Manejador concreto para la consulta de lista paginada de Categorías.
/// Implementa la lógica específica de filtrado y ordenación.
/// </summary>
public sealed class GetConceptosPagedListQueryHandler
    : GetPagedListQueryHandler<Concepto, ConceptoDto, GetConceptosPagedListQuery>
{
    public GetConceptosPagedListQueryHandler(
        IReadRepository<Concepto> repository,
        ICacheService cacheService)
        : base(repository, cacheService)
    {
        // No se necesita lógica adicional en el constructor.
    }

    /// <summary>
    /// **Implementación de la lógica de aplicación de filtros y ordenación.**
    /// </summary>
    protected override IQueryable<Concepto> ApplyQuery(GetConceptosPagedListQuery query)
    {
        IQueryable<Concepto> queryable = GetQueryBase();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            // Guardamos el término para evitar duplicarlo
            string searchTerm = query.SearchTerm;

            // Filtra por Nombre o Descripción
            queryable = queryable.Where(c =>
                EF.Functions.Like(c.Nombre.Value, $"%{searchTerm}%")
            );

        }

        if (!string.IsNullOrWhiteSpace(query.SortColumn))
        {
            // Utilizamos el mismo tipo de selector para Ordenar
            Expression<Func<Concepto, object>> keySelector = query.SortColumn.ToLower() switch
            {
                "nombre" => c => c.Nombre.Value,
                "id" => c => c.Id,
                _ => c => c.Id
            };

            queryable = query.SortOrder?.ToLower() == "desc"
                ? queryable.OrderByDescending(keySelector)
                : queryable.OrderBy(keySelector);
        }
        else
        {
            queryable = queryable.OrderBy(c => c.Id);
        }

        return queryable;
    }
}