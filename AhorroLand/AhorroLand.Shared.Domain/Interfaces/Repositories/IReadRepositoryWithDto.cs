using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Results;

namespace AhorroLand.Shared.Domain.Interfaces.Repositories
{
    /// <summary>
    /// ?? Interfaz extendida para repositorios que trabajan con DTOs optimizados.
    /// Permite obtener DTOs directamente desde la base de datos sin mapeo intermedio.
    /// </summary>
    public interface IReadRepositoryWithDto<T, TDto> : IReadRepository<T>
        where T : AbsEntity
        where TDto : class
    {
        /// <summary>
        /// ?? OPTIMIZADO: Obtiene una página de DTOs directamente desde la base de datos.
        /// Evita el mapeo de Value Objects y mejora el rendimiento.
        /// </summary>
        Task<PagedList<TDto>> GetPagedReadModelsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// ?? OPTIMIZADO: Obtiene una página de DTOs filtrada por usuario.
        /// Usa índices en la base de datos para máximo rendimiento.
        /// </summary>
        Task<PagedList<TDto>> GetPagedReadModelsByUserAsync(Guid usuarioId, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// ?? OPTIMIZADO: Obtiene todos los DTOs directamente desde la base de datos.
        /// </summary>
        Task<IEnumerable<TDto>> GetAllReadModelsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// ?? OPTIMIZADO: Obtiene un DTO por ID directamente desde la base de datos.
        /// </summary>
        Task<TDto?> GetReadModelByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
