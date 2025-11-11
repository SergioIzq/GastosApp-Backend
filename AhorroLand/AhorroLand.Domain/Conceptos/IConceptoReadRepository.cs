using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain;

public interface IConceptoReadRepository : IReadRepository<Concepto>
{
    /// <summary>
    /// Verifica si ya existe un concepto con el mismo nombre para un usuario.
    /// </summary>
    Task<bool> ExistsWithSameNameAsync(Nombre nombre, UsuarioId usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si ya existe un concepto con el mismo nombre para un usuario, excluyendo un concepto específico (para updates).
    /// </summary>
    Task<bool> ExistsWithSameNameExceptAsync(Nombre nombre, UsuarioId usuarioId, Guid excludeId, CancellationToken cancellationToken = default);
}