using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Results;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Interfaces;

/// <summary>
/// Interfaz base para todos los Query Handlers.
/// ✅ OPTIMIZADO: Solo mantiene el método de paginación con IQueryable (rara vez usado).
/// Los handlers deben usar IReadRepositoryWithDto directamente para consultas optimizadas.
/// </summary>
/// <typeparam name="TEntity">La entidad raíz que maneja el handler.</typeparam>
public interface IQueryHandlerBase<TEntity>
    where TEntity : AbsEntity
{
    /// <summary>
    /// Paginación genérica con IQueryable.
    /// La mayoría de los casos usan GetPagedListQueryHandler que llama a GetPagedReadModelsAsync.
    /// </summary>
    Task<Result<PagedList<TResult>>> GetPagedListAsync<TResult>(
        IQueryable<TResult> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TResult : class;
}
