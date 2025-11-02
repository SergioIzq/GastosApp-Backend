using AhorroLand.Shared.Domain.Abstractions;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories;

/// <summary>
/// Define las operaciones de escritura: Creación, Actualización y Eliminación.
/// </summary>
public interface IWriteRepository<T> where T : AbsEntity
{
    void Add(T entity);

    // Retorna T, permitiendo a la capa de aplicación acceder a propiedades generadas (e.g., marcas de tiempo).
    T Update(T entity);

    // Retorna bool para indicar si la operación de marcado fue exitosa.
    bool Delete(T entity);
}
