using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Results;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Define operaciones de lectura.
    /// Prioriza la velocidad usando AsNoTracking().
    /// ✅ OPTIMIZADO: Incluye paginación a nivel de base de datos.
    /// </summary>
    public interface IReadRepository<T> where T : AbsEntity
    {
        /// <summary>
        /// ⚠️ DEPRECADO: Usa GetPagedAsync para evitar cargar toda la tabla en memoria.
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtener por ID con opción a AsNoTracking.
        /// </summary>
        Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// 🚀 OPTIMIZADO: Obtiene una página de datos directamente desde la base de datos.
        /// </summary>
        Task<PagedList<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
