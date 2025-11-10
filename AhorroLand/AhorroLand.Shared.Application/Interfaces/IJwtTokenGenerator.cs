using AhorroLand.Domain;

namespace AhorroLand.Shared.Application.Interfaces;

/// <summary>
/// Interfaz para el generador de tokens JWT.
/// Encapsula la lógica de generación de tokens de autenticación.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Genera un token JWT para un usuario autenticado.
    /// </summary>
    /// <param name="usuario">El usuario para el cual generar el token.</param>
    /// <returns>Una tupla con el token y la fecha de expiración.</returns>
    (string Token, DateTime ExpiresAt) GenerateToken(Usuario usuario);
}
