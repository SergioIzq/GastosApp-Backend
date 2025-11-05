using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Interfaces;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts;

/// <summary>
/// Proporciona métodos base para manejar consultas de solo lectura (Queries).
/// Este handler prioriza la velocidad inyectando solo el IReadRepository y utilizando IQueryable.
/// </summary>
/// <typeparam name="TEntity">El tipo de entidad que el query handler consulta, debe heredar de AbsEntity.</typeparam>
public abstract class AbsQueryHandler<TEntity> : IQueryHandlerBase<TEntity>
    where TEntity : AbsEntity
{
    protected readonly IReadRepository<TEntity> _repository;
    protected readonly ICacheService _cacheService;

    /// <summary>
    /// Inicializa una nueva instancia de la clase AbsQueryHandler.
    /// </summary>
    /// <param name="repository">El repositorio con métodos de solo lectura (IReadRepository).</param>
    public AbsQueryHandler(IReadRepository<TEntity> repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Obtiene una entidad por su identificador único de forma asíncrona.
    /// Utiliza 'AsNoTracking' para optimizar la velocidad de la consulta.
    /// </summary>
    /// <param name="id">El identificador único (Guid) de la entidad a buscar.</param>
    /// <param name="cancellationToken">Token para monitorear peticiones de cancelación.</param>
    /// <returns>Un Result que contiene la entidad si se encuentra, o Error.NotFound si no existe.</returns>
    public async Task<Result<TEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"{typeof(TEntity).Name}:{id}";

        // 1. Intentar obtener de la caché
        var cachedEntity = await _cacheService.GetAsync<TEntity>(cacheKey);
        if (cachedEntity != null)
        {
            return Result.Success(cachedEntity);
        }

        // 2. Si no está en caché, leer de la DB
        try
        {
            var entity = await _repository.GetByIdAsync(id, asNoTracking: true, cancellationToken);

            if (entity is not null)
            {
                // 3. Guardar en caché con una expiración razonable (ej: 30 minutos)
                await _cacheService.SetAsync(
                    cacheKey,
                    entity,
                    absoluteExpiration: TimeSpan.FromMinutes(30)
                );
                return Result.Success(entity);
            }

            return Result.Failure<TEntity>(Error.NotFound($"Entidad con ID '{id}' de tipo {typeof(TEntity).Name} no fue encontrada."));
        }
        catch (Exception ex)
        {
            string detail = $"Error inesperado al generar el listado paginado: {ex.Message}";
            return Result.Failure<TEntity>(Error.Failure("Error.Read", "Fallo de lectura inesperado", detail));
        }
    }

    /// <summary>
    /// Genera un listado paginado en el servidor a partir de una consulta IQueryable.
    /// Esto es fundamental para una paginación eficiente.
    /// </summary>
    /// <typeparam name="TResult">El DTO o tipo al que se proyectarán los resultados.</typeparam>
    /// <param name="query">La consulta IQueryable (ya filtrada y proyectada) sobre la que se aplicará la paginación.</param>
    /// <param name="page">El número de página (base 1).</param>
    /// <param name="pageSize">El tamaño de los elementos por página.</param>
    /// <param name="cancellationToken">Token para monitorear peticiones de cancelación.</param>
    /// <returns>Un Result que contiene un objeto PagedList<TResult>.</returns>
    // Dentro de AbsQueryHandler<TEntity>
    public async Task<Result<PagedList<TResult>>> GetPagedListAsync<TResult>(
        IQueryable<TResult> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TResult : class
    {
        // 1. Validar la paginación (igual que antes)
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        try
        {
            // 4. Obtener el conteo total ANTES de aplicar Skip/Take
            int totalCount = await query.CountAsync(cancellationToken);

            // 5. Aplicar paginación en el servidor y obtener elementos
            List<TResult> items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // 6. Devolver la lista paginada
            var pagedResult = new PagedList<TResult>(items, page, pageSize, totalCount);

            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            string detail = $"Error inesperado al generar el listado paginado: {ex.Message}";
            return Result.Failure<PagedList<TResult>>(Error.Failure("Error.Paging", "Fallo de paginación", detail));
        }
    }
}