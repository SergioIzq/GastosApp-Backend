using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Results;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Interfaces;

/// <summary>
/// Interfaz base para todos los Query Handlers.
/// Define los contratos para la lectura de datos comunes (por ID y Paginación).
/// </summary>
/// <typeparam name="TEntity">La entidad raíz que maneja el handler.</typeparam>
public interface IQueryHandlerBase<TEntity>
    where TEntity : AbsEntity
{
    // El método GetByIdAsync debe ser público para ser parte de una interfaz.
    Task<Result<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // El método GetPagedListAsync debe ser público para ser parte de una interfaz.
    Task<Result<PagedList<TResult>>> GetPagedListAsync<TResult>(
        IQueryable<TResult> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TResult : class;
}
