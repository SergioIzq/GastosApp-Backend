using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain;

/// <summary>
/// Interfaz para el repositorio de lectura de Clientes.
/// </summary>
public interface IClienteReadRepository : IReadRepository<Cliente>
{
    /// <summary>
    /// Verifica si ya existe un cliente con el mismo nombre para un usuario.
    /// </summary>
    Task<bool> ExistsWithSameNameAsync(Nombre nombre, UsuarioId usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si ya existe un cliente con el mismo nombre para un usuario, excluyendo un cliente específico (para updates).
    /// </summary>
    Task<bool> ExistsWithSameNameExceptAsync(Nombre nombre, UsuarioId usuarioId, Guid excludeId, CancellationToken cancellationToken = default);
}