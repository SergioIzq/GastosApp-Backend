using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Interfaces;

/// <summary>
/// Interfaz base para todos los Command Handlers.
/// Define los contratos para las operaciones de escritura CUD (Create, Update, Delete).
/// </summary>
/// <typeparam name="TEntity">El tipo de entidad que el command handler manipula, debe heredar de AbsEntity.</typeparam>
public interface IAbsCommandHandlerBase<TEntity>
    where TEntity : AbsEntity
{
    /// <summary>
    /// Crea una nueva entidad y persiste los cambios.
    /// </summary>
    Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca la entidad como modificada y persiste los cambios.
    /// </summary>
    Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca la entidad para su eliminación y persiste los cambios.
    /// </summary>
    Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
