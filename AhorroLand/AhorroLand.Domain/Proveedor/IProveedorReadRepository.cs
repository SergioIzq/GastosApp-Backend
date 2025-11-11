using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain
{
    public interface IProveedorReadRepository : IReadRepository<Proveedor>
    {
        /// <summary>
        /// Verifica si ya existe un proveedor con el mismo nombre para un usuario.
        /// </summary>
        Task<bool> ExistsWithSameNameAsync(Nombre nombre, UsuarioId usuarioId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si ya existe un proveedor con el mismo nombre para un usuario, excluyendo un proveedor específico (para updates).
        /// </summary>
        Task<bool> ExistsWithSameNameExceptAsync(Nombre nombre, UsuarioId usuarioId, Guid excludeId, CancellationToken cancellationToken = default);
    }
}