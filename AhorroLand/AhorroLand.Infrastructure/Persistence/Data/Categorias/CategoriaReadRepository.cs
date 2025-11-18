using AhorroLand.Domain;
using AhorroLand.Infrastructure.Persistence.Query;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.ValueObjects;
using Dapper;

namespace AhorroLand.Infrastructure.Persistence.Data.Categorias
{
    public class CategoriaReadRepository : AbsReadRepository<Categoria, CategoriaDto>, ICategoriaReadRepository
    {
        public CategoriaReadRepository(IDbConnectionFactory dbConnectionFactory)
            : base(dbConnectionFactory, "categorias")
        {
        }

        public async Task<bool> ExistsWithSameNameAsync(Nombre nombre, UsuarioId usuarioId, CancellationToken cancellationToken = default)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            const string sql = @"
         SELECT COUNT(1) 
  FROM Categorias 
    WHERE Nombre = @Nombre AND IdUsuario = @UsuarioId";

            var count = await connection.ExecuteScalarAsync<int>(
          sql,
    new { Nombre = nombre.Value, UsuarioId = usuarioId.Value });

            return count > 0;
        }

        public async Task<bool> ExistsWithSameNameExceptAsync(Nombre nombre, UsuarioId usuarioId, Guid excludeId, CancellationToken cancellationToken = default)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            const string sql = @"
     SELECT COUNT(1) 
          FROM Categorias 
    WHERE Nombre = @Nombre AND IdUsuario = @UsuarioId AND Id != @ExcludeId";

            var count = await connection.ExecuteScalarAsync<int>(
      sql,
  new { Nombre = nombre.Value, UsuarioId = usuarioId.Value, ExcludeId = excludeId });

            return count > 0;
        }
    }
}