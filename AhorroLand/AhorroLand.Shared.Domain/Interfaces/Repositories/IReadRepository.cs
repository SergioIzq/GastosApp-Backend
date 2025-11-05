using AhorroLand.Shared.Domain.Abstractions;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Define operaciones de lectura.
    /// Prioriza la velocidad usando AsNoTracking().
    /// </summary>
    public interface IReadRepository<T> where T : AbsEntity
    {
        // Permite al QueryHandler aplicar proyecciones (Select) antes de ejecutar.
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        // Obtener por ID con opción a AsNoTracking.
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default);
    }
}
