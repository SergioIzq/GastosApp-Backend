using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain
{
    public interface IPersonaReadRepository : IReadRepository<Persona>
    {
        /// <summary>
        /// Verifica si ya existe una persona con el mismo nombre para un usuario.
        /// </summary>
        Task<bool> ExistsWithSameNameAsync(Nombre nombre, UsuarioId usuarioId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si ya existe una persona con el mismo nombre para un usuario, excluyendo una persona específica (para updates).
        /// </summary>
        Task<bool> ExistsWithSameNameExceptAsync(Nombre nombre, UsuarioId usuarioId, Guid excludeId, CancellationToken cancellationToken = default);
    }
}
