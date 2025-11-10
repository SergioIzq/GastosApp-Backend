using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Domain;

public interface IUsuarioReadRepository : IReadRepository<Usuario>
{
    /// <summary>
    /// Obtiene un usuario por su correo electrónico.
    /// </summary>
    Task<Usuario?> GetByEmailAsync(Email correo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un usuario por su token de confirmación.
    /// </summary>
    Task<Usuario?> GetByConfirmationTokenAsync(string token, CancellationToken cancellationToken = default);
}