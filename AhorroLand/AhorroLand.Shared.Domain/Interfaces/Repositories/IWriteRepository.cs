using AhorroLand.Shared.Domain.Abstractions;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories;

/// <summary>
/// Define las operaciones de escritura: Creación, Actualización y Eliminación.
/// </summary>
public interface IWriteRepository<T> where T : AbsEntity
{
    /// <summary>
    /// Obtiene una entidad por ID con tracking habilitado (para Commands).
    /// </summary>
    /// <param name="id">El ID de la entidad a buscar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>La entidad con tracking o null si no existe</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(T entity);

    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    /// <summary>
    /// Actualiza una entidad de forma asíncrona con validaciones.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    void Delete(T entity);
}
