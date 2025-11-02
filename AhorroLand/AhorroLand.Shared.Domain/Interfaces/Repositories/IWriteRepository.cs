using AhorroLand.Shared.Domain.Abstractions;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories;

/// <summary>
/// Define las operaciones de escritura: Creación, Actualización y Eliminación.
/// </summary>
public interface IWriteRepository<T> where T : AbsEntity
{
    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}
