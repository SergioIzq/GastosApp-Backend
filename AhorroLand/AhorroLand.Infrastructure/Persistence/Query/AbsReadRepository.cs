using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Dapper;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace AhorroLand.Infrastructure.Persistence.Query
{
    /// <summary>
    /// Repositorio de lectura base abstracto implementado con Dapper.
    /// ✅ OPTIMIZADO: Reduce allocations, usa caching opcional y mejora queries.
    /// </summary>
    /// <typeparam name="T">La entidad que debe heredar de AbsEntity</typeparam>
    /// <typeparam name="TReadModel">El modelo de lectura (DTO plano para Dapper)</typeparam>
    public abstract class AbsReadRepository<T, TReadModel> : IReadRepository<T> where T : AbsEntity
    {
        protected readonly IDbConnectionFactory _dbConnectionFactory;
        protected readonly string _tableName;
        private readonly IDistributedCache? _cache;

        protected AbsReadRepository(
            IDbConnectionFactory dbConnectionFactory, 
            string tableName,
            IDistributedCache? cache = null)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _tableName = tableName;
            _cache = cache;
        }

        /// <summary>
        /// 🔥 OVERRIDE OPCIONAL: Personaliza el query de GetById si necesitas columnas específicas.
        /// Por defecto usa un query genérico.
        /// </summary>
        protected virtual string BuildGetByIdQuery()
        {
            return $@"
                SELECT 
                    BIN_TO_UUID(id) as Id,
                    BIN_TO_UUID(usuario_id) as UsuarioId,
                    nombre as Nombre,
                    fecha_creacion as FechaCreacion
                FROM {_tableName} 
                WHERE id = UUID_TO_BIN(@id)";
        }

        /// <summary>
        /// 🔥 OVERRIDE OPCIONAL: Personaliza el query de GetAll si necesitas columnas específicas.
        /// </summary>
        protected virtual string BuildGetAllQuery()
        {
            return $@"
                SELECT 
                    BIN_TO_UUID(id) as Id,
                    BIN_TO_UUID(usuario_id) as UsuarioId,
                    nombre as Nombre,
                    fecha_creacion as FechaCreacion
                FROM {_tableName}";
        }

        /// <summary>
        /// 🚀 OPTIMIZADO: Obtiene el ReadModel con cache opcional.
        /// </summary>
        public virtual async Task<TReadModel?> GetReadModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // 1. Intentar obtener del cache
            if (_cache != null)
            {
                var cacheKey = $"{_tableName}:{id}";
                var cachedData = await _cache.GetAsync(cacheKey, cancellationToken);
                
                if (cachedData != null)
                {
                    return JsonSerializer.Deserialize<TReadModel>(cachedData);
                }
            }

            // 2. Query a la base de datos
            using var connection = _dbConnectionFactory.CreateConnection();

            // 🔥 OPTIMIZACIÓN: Reutilizar DynamicParameters
            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            var sql = BuildGetByIdQuery();
            var result = await connection.QueryFirstOrDefaultAsync<TReadModel>(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)
            );

            // 3. Guardar en cache si existe
            if (result != null && _cache != null)
            {
                var cacheKey = $"{_tableName}:{id}";
                var serialized = JsonSerializer.SerializeToUtf8Bytes(result);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                await _cache.SetAsync(cacheKey, serialized, options, cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// ⚠️ SOLO para Commands que necesiten la entidad de dominio con lógica de negocio.
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var readModel = await GetReadModelByIdAsync(id, cancellationToken);

            if (readModel is null)
                return null;

            // Solo mapear cuando realmente se necesite la entidad
            return readModel.Adapt<T>();
        }

        /// <summary>
        /// 🚀 OPTIMIZADO: Retorna DTOs directamente sin allocations extras.
        /// </summary>
        public virtual async Task<IEnumerable<TReadModel>> GetAllReadModelsAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = BuildGetAllQuery();
            
            return await connection.QueryAsync<TReadModel>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );
        }

        /// <summary>
        /// ⚠️ DEPRECADO: Usa GetAllReadModelsAsync para mejor rendimiento.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var readModels = await GetAllReadModelsAsync(cancellationToken);
                        
            var list = new List<T>(readModels is ICollection<TReadModel> col ? col.Count : 0);
            foreach (var rm in readModels)
            {
                list.Add(rm.Adapt<T>());
            }
            return list;
        }
    }
}