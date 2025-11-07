using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Interfaces;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts;

/// <summary>
/// Proporciona métodos base para manejar consultas de solo lectura (Queries).
/// </summary>
public abstract class AbsQueryHandler<TEntity> : IQueryHandlerBase<TEntity>
    where TEntity : AbsEntity
{
    protected readonly IReadRepository<TEntity> _repository;
    protected readonly ICacheService _cacheService;

    public AbsQueryHandler(IReadRepository<TEntity> repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    /// <summary>
    /// ⚠️ DEPRECADO: Usa GetByIdWithCacheAsync<TDto> para mejor rendimiento.
    /// </summary>
    public async Task<Result<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"{typeof(TEntity).Name}:{id}";

        var cachedEntity = await _cacheService.GetAsync<TEntity>(cacheKey);
        if (cachedEntity != null)
        {
            return Result.Success(cachedEntity);
        }

        try
        {
            var entity = await _repository.GetByIdAsync(id, asNoTracking: true, cancellationToken);

            if (entity is not null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    entity,
                    absoluteExpiration: TimeSpan.FromMinutes(30)
                );
                return Result.Success(entity);
            }

            return Result.Failure<TEntity>(Error.NotFound($"Entidad con ID '{id}' de tipo {typeof(TEntity).Name} no fue encontrada."));
        }
        catch (Exception ex)
        {
            string detail = $"Error inesperado: {ex.Message}";
            return Result.Failure<TEntity>(Error.Failure("Error.Read", "Fallo de lectura", detail));
        }
    }

    /// <summary>
    /// 🚀 OPTIMIZADO: Obtiene el DTO directamente desde el repositorio sin mapeo intermedio.
    /// </summary>
    public async Task<Result<TDto>> GetByIdWithCacheAsync<TDto>(Guid id, CancellationToken cancellationToken = default)
        where TDto : class
    {
        string cacheKey = $"{typeof(TEntity).Name}:{id}";

        // 1. Intentar obtener el DTO de la caché
        var cachedDto = await _cacheService.GetAsync<TDto>(cacheKey);
        if (cachedDto != null)
        {
            return Result.Success(cachedDto);
        }

        // 2. Obtener la entidad desde el repositorio
        try
        {
            var entity = await _repository.GetByIdAsync(id, asNoTracking: true, cancellationToken);

            if (entity is null)
            {
                return Result.Failure<TDto>(Error.NotFound(
                    $"Entidad con ID '{id}' de tipo {typeof(TEntity).Name} no fue encontrada."));
            }

            // 3. Mapear a DTO UNA SOLA VEZ usando Mapster
            var dto = entity.Adapt<TDto>();

            // 4. Cachear el DTO directamente
            await _cacheService.SetAsync(
                cacheKey,
                dto,
                absoluteExpiration: TimeSpan.FromMinutes(30)
            );

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            string detail = $"Error inesperado al obtener por ID: {ex.Message}";
            return Result.Failure<TDto>(Error.Failure("Error.Read", "Fallo de lectura", detail));
        }
    }

    public async Task<Result<PagedList<TResult>>> GetPagedListAsync<TResult>(
        IQueryable<TResult> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        try
        {
            int totalCount = await query.CountAsync(cancellationToken);

            List<TResult> items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedList<TResult>(items, page, pageSize, totalCount);

            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            string detail = $"Error inesperado al generar el listado paginado: {ex.Message}";
            return Result.Failure<PagedList<TResult>>(Error.Failure("Error.Paging", "Fallo de paginación", detail));
        }
    }
}