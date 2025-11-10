using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Dapper;
using Mapster;
using static Dapper.SqlMapper;

namespace AhorroLand.Infrastructure.Persistence.Query
{
    /// <summary>
    /// Repositorio de lectura base abstracto implementado con Dapper.
    /// </summary>
    /// <typeparam name="T">La entidad que debe heredar de AbsEntity</typeparam>
    /// <typeparam name="TReadModel">El modelo de lectura (DTO plano para Dapper)</typeparam>
    public abstract class AbsReadRepository<T, TReadModel> : IReadRepository<T> where T : AbsEntity
    {
        protected readonly IDbConnectionFactory _dbConnectionFactory;
        protected readonly string _tableName;

        protected AbsReadRepository(IDbConnectionFactory dbConnectionFactory, string tableName)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _tableName = tableName;
        }

        /// <summary>
        /// 🚀 OPTIMIZADO: Obtiene el ReadModel directamente sin mapeo.
        /// </summary>
        public virtual async Task<TReadModel?> GetReadModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            // 🚀 OPTIMIZACIÓN: Usar pooling de DynamicParameters
            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            var sql = $@"
        SELECT 
       BIN_TO_UUID(id) as Id,
     BIN_TO_UUID(usuario_id) as UsuarioId,
     nombre as Nombre,
  fecha_creacion as FechaCreacion
   FROM {_tableName} 
             WHERE id = UUID_TO_BIN(@id)";

            // ✅ Retornar directamente el DTO sin mapeo
            return await connection.QueryFirstOrDefaultAsync<TReadModel>(
      new CommandDefinition(sql, parameters, cancellationToken: cancellationToken)
            );
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
        /// 🚀 OPTIMIZADO: Retorna DTOs directamente.
        /// </summary>
        public virtual async Task<IEnumerable<TReadModel>> GetAllReadModelsAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = $@"
      SELECT 
      BIN_TO_UUID(id) as Id,
          BIN_TO_UUID(usuario_id) as UsuarioId,
              nombre as Nombre,
    fecha_creacion as FechaCreacion
         FROM {_tableName}";

            // ✅ Retornar DTOs directamente
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
            return readModels.Select(rm => rm.Adapt<T>());
        }
    }
}