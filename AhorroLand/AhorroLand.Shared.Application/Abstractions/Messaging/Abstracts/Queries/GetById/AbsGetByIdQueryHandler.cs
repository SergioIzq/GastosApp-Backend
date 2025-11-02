using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;

public abstract class GetByIdQueryHandler<TEntity, TDto, TQuery>
    : AbsQueryHandler<TEntity>, IRequestHandler<TQuery, Result<TDto>>
    where TEntity : AbsEntity
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
        // 1. Usar el método base, que maneja la caché y la búsqueda de la entidad.
        // Retorna Result<TEntity>
        var entityResult = await GetByIdAsync(query.Id, cancellationToken);

        if (entityResult.IsFailure)
        {
            // Si falla (ej: NotFound), devuelve el error directamente con el tipo TDto
            return Result.Failure<TDto>(entityResult.Error);
        }

        // 2. Mapeo a DTO (Usando Mapster)
        var dto = entityResult.Value.Adapt<TDto>();

        return Result.Success(dto);
    }
}