using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain
{
    public interface ICategoriaReadRepository : IReadRepository<Categoria>
    {
        /// <summary>
        /// Verifica si ya existe una categoría con el mismo nombre para un usuario.
        /// </summary>
        Task<bool> ExistsWithSameNameAsync(Nombre nombre, UsuarioId usuarioId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si ya existe una categoría con el mismo nombre para un usuario, excluyendo una categoría específica (para updates).
        /// </summary>
        Task<bool> ExistsWithSameNameExceptAsync(Nombre nombre, UsuarioId usuarioId, Guid excludeId, CancellationToken cancellationToken = default);
    }
}