using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Interfaces;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts;

/// <summary>
/// Proporciona métodos base para manejar comandos de escritura (CRUD: C, U, D) de forma asíncrona.
/// Utiliza IWriteRepository e IUnitOfWork para asegurar la segregación de responsabilidades.
/// </summary>
/// <typeparam name="TEntity">El tipo de entidad que el command handler manipula, debe heredar de AbsEntity.</typeparam>
public abstract class AbsCommandHandler<TEntity> : IAbsCommandHandlerBase<TEntity>
    where TEntity : AbsEntity
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IWriteRepository<TEntity> _writeRepository;
    protected readonly ICacheService _cacheService;

    /// <summary>
    /// Inicializa una nueva instancia de la clase AbsCommandHandler.
    /// </summary>
    /// <param name="unitOfWork">La unidad de trabajo para gestionar la persistencia de cambios.</param>
    /// <param name="writeRepository">El repositorio con métodos de escritura (Add, Update, Delete).</param>
    public AbsCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<TEntity> writeRepository,
        ICacheService cacheService
        )
    {
        _unitOfWork = unitOfWork;
        _writeRepository = writeRepository;
        _cacheService = cacheService;
    }

    // --- Métodos CUD (Create, Update, Delete) ---

    /// <summary>
    /// Añade la entidad al repositorio y persiste los cambios.
    /// </summary>
    /// <param name="entity">La entidad a crear.</param>
    /// <param name="cancellationToken">Token para monitorear peticiones de cancelación.</param>
    /// <returns>Un Result que contiene la entidad creada en caso de éxito, o Error.Conflict si falla.</returns>
    public async Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _writeRepository.Add(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await InvalidateIndividualCacheAsync(entity.Id);

            return Result.Success(entity);
        }
        catch (Exception ex)
        {
            string detail = $"Error inesperado al crear {typeof(TEntity).Name}: {ex.Message}";
            return Result.Failure<TEntity>(Error.Conflict(detail));
        }
    }

    /// <summary>
    /// Marca la entidad como modificada y persiste los cambios.
    /// </summary>
    /// <param name="entity">La entidad a actualizar.</param>
    /// <param name="cancellationToken">Token para monitorear peticiones de cancelación.</param>
    /// <returns>Un Result de éxito si la actualización fue exitosa, o Error.UpdateFailure si falla.</returns>
    public async Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _writeRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await InvalidateIndividualCacheAsync(entity.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            string detail = $"Error al actualizar {typeof(TEntity).Name}: {ex.Message}";
            return Result.Failure(Error.UpdateFailure(detail));
        }
    }

    /// <summary>
    /// Marca la entidad para su eliminación y persiste los cambios.
    /// </summary>
    /// <param name="entity">La entidad a eliminar.</param>
    /// <param name="cancellationToken">Token para monitorear peticiones de cancelación.</param>
    /// <returns>Un Result de éxito si la eliminación fue exitosa, o Error.DeleteFailure si falla.</returns>
    public async Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _writeRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await InvalidateIndividualCacheAsync(entity.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            string detail = $"Error al eliminar {typeof(TEntity).Name}: {ex.Message}";
            return Result.Failure(Error.DeleteFailure(detail));
        }
    }

    /// <summary>
    /// Invalida la clave de caché individual de una entidad modificada o eliminada.
    /// </summary>
    protected async Task InvalidateIndividualCacheAsync(Guid id)
    {
        // Esta clave debe coincidir exactamente con la clave usada en AbsQueryHandler.GetByIdAsync
        string cacheKey = $"{typeof(TEntity).Name}:{id}";
        await _cacheService.RemoveAsync(cacheKey);
    }
}