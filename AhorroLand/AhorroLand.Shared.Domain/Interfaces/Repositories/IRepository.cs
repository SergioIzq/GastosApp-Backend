using AhorroLand.Shared.Domain.Abstractions;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Define operaciones de escritura. Hereda lectura para comandos.
    /// Las implementaciones deben asegurar el seguimiento de cambios (Tracking) si se requiere.
    /// </summary>
    public interface IRepository<T> : IReadOnlyRepository<T> where T : AbsEntity
    {
        void Add(T entity);
        T Update(T entity);
        bool Delete(T entity);
    }
}
