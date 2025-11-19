using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Interfaces;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts;

/// <summary>
/// Proporciona métodos base para manejar consultas de solo lectura (Queries).
/// ✅ OPTIMIZADO: Usa DTOs directamente para evitar mapeo innecesario.
/// Los handlers concretos deben usar IReadRepositoryWithDto para obtener DTOs optimizados.
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
    /// 🚀 OPTIMIZADO: Para paginación con IQueryable (rara vez usado ahora).
    /// La mayoría de los casos usan GetPagedListQueryHandler que llama a GetPagedReadModelsAsync.
    /// </summary>
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