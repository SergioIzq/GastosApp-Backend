using AhorroLand.Application.Features.Categorias.Queries;
using AhorroLand.Domain.Clientes;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AhorroLand.Application.Features.Clientes.Queries;

/// <summary>
/// Manejador concreto para la consulta de lista paginada de Categorías.
/// Implementa la lógica específica de filtrado y ordenación.
/// </summary>
public sealed class GetClientesPagedListQueryHandler
    : GetPagedListQueryHandler<Cliente, ClienteDto, GetClientesPagedListQuery>
{
    public GetClientesPagedListQueryHandler(
        IReadOnlyRepository<Cliente> repository,
        ICacheService cacheService)
        : base(repository, cacheService)
    {
        // No se necesita lógica adicional en el constructor.
    }

    /// <summary>
    /// **Implementación de la lógica de aplicación de filtros y ordenación.**
    /// </summary>
    protected override IQueryable<Cliente> ApplyQuery(GetClientesPagedListQuery query)
    {
        IQueryable<Cliente> queryable = GetQueryBase();

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
            Expression<Func<Cliente, object>> keySelector = query.SortColumn.ToLower() switch
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