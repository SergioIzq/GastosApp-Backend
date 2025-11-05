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

        //// 🔑 MÉTODO ABSTRACTO: Obliga al handler concreto a aplicar filtros y proyecciones
        //protected abstract IQueryable<TEntity> ApplyQuery(TQuery query);

        public async Task<Result<PagedList<TDto>>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            // 1. ¡ADVERTENCIA! Esto trae TODA la tabla a la memoria.
            IEnumerable<TEntity> baseQuery = await _repository.GetAllAsync();

            // 2. Usa .Adapt<T>() para mapear la lista en memoria (no .ProjectToType)
            IEnumerable<TDto> projectedList = baseQuery.Adapt<IEnumerable<TDto>>();

            // 3. Debes crear un método 'GetPagedListFromMemory' que pagine
            //    una lista 'IEnumerable' en lugar de un 'IQueryable'.
            var pagedResult = GetPagedListFromMemory(
                projectedList,
                query.Page,
                query.PageSize
            );

            return Result.Success(pagedResult);
        }

        // Necesitarás un helper como este:
        private PagedList<TDto> GetPagedListFromMemory(IEnumerable<TDto> source, int page, int pageSize)
        {
            var totalCount = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<TDto>(items, totalCount, page, pageSize);
        }
    }
}