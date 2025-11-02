using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.Results;
using Mapster;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries
{
    public abstract class GetPagedListQueryHandler<TEntity, TDto, TQuery>
        : AbsQueryHandler<TEntity>, IRequestHandler<TQuery, Result<PagedList<TDto>>>
        where TEntity : AbsEntity
        where TQuery : AbsGetPagedListQuery<TEntity, TDto>
        where TDto : class
    {
        public GetPagedListQueryHandler(
            IReadRepository<TEntity> repository,
            ICacheService cacheService)
            : base(repository, cacheService)
        {
        }

        // 🔑 MÉTODO ABSTRACTO: Obliga al handler concreto a aplicar filtros y proyecciones
        protected abstract IQueryable<TEntity> ApplyQuery(TQuery query);

        public async Task<Result<PagedList<TDto>>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            // 1. Obtener la consulta base IQueryable (AsNoTracking)
            IQueryable<TEntity> baseQuery = GetQueryBase();

            // 2. Aplicar filtros/orden/Eager Loading (Definido por la clase concreta)
            IQueryable<TEntity> filteredQuery = ApplyQuery(query);

            // 3. Proyección a DTO (Mapster puede proyectar en IQueryable)
            // Esto convierte el IQueryable<TEntity> en IQueryable<TDto> en el servidor.
            IQueryable<TDto> projectedQuery = filteredQuery.ProjectToType<TDto>();

            // 4. Usar el método base para aplicar la paginación eficiente
            var pagedResult = await GetPagedListAsync(
                projectedQuery,
                query.Page,
                query.PageSize,
                cancellationToken
            );

            // El resultado ya es Result<PagedList<TDto>>
            return pagedResult;
        }
    }
}