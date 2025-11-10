using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;

public abstract class GetByIdQueryHandler<TEntity, TDto, TQuery>
 : AbsQueryHandler<TEntity>, IRequestHandler<TQuery, Result<TDto>>
    where TEntity : AbsEntity
    where TDto : class
  where TQuery : AbsGetByIdQuery<TEntity, TDto>
{
    public GetByIdQueryHandler(
     IReadRepository<TEntity> repository,
        ICacheService cacheService)
   : base(repository, cacheService)
    {
    }

    public async Task<Result<TDto>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        // 🚀 OPTIMIZADO: Usa el método que cachea DTOs directamente
        return await GetByIdWithCacheAsync<TDto>(query.Id, cancellationToken);
    }
}