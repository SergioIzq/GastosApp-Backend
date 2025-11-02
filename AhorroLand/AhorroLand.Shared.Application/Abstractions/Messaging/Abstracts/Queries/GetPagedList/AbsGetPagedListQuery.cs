using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Results;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries
{
    /// <summary>
    /// Consulta base genérica para obtener listados paginados.
    /// </summary>
    /// <typeparam name="TEntity">La Entidad de Dominio base (para el repositorio).</typeparam>
    /// <typeparam name="TDto">El DTO que representará cada elemento de la lista.</typeparam>
    public abstract record AbsGetPagedListQuery<TEntity, TDto>(int Page, int PageSize) : IRequest<Result<PagedList<TDto>>>
        where TEntity : AbsEntity;
}