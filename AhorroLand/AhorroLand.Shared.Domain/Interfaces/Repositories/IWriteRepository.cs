using AhorroLand.Shared.Domain.Abstractions;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories;

/// <summary>
/// Define las operaciones de escritura: Creación, Actualización y Eliminación.
/// </summary>
public interface IWriteRepository<T> where T : AbsEntity
{
    void Add(T entity);

    Task CreateAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    /// <summary>
    /// Actualiza una entidad de forma asíncrona con validaciones.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    void Delete(T entity);
}
