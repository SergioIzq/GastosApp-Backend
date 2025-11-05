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
    public abstract class AbsReadRepository<T, TReadModel> : IReadRepository<T> where T : AbsEntity
    {
        // Usa la fábrica de conexiones, no un DbContext
        protected readonly IDbConnectionFactory _dbConnectionFactory;

        // Dapper necesita el nombre de la tabla, EF Core no.
        protected readonly string _tableName;

        /// <summary>
        /// Constructor base.
        /// </summary>
        /// <param name="dbConnectionFactory">La fábrica para crear conexiones.</param>
        /// <param name="tableName">El nombre de la tabla que consultará este repositorio.</param>
        protected AbsReadRepository(IDbConnectionFactory dbConnectionFactory, string tableName)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _tableName = tableName;
        }

        /// <summary>
        /// Obtiene una entidad por su Id.
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            // 'asNoTracking' es irrelevante para Dapper, ya que nunca rastrea entidades.

            // Crea y abre una nueva conexión para esta consulta
            using var connection = _dbConnectionFactory.CreateConnection();

            var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<TReadModel>(
                    new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)
                );

            // Si no se encuentra nada, devolvemos null
            if (result is null)
                return null;

            var entity = result.Adapt<T>();

            return entity;
        }

        /// <summary>
        /// ADVERTENCIA: Esta implementación trae TODA la tabla a memoria.
        /// Ver la nota de advertencia sobre IQueryable.
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var sql = $"SELECT * FROM {_tableName}";

            return await connection.QueryAsync<T>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );
        }
    }
}