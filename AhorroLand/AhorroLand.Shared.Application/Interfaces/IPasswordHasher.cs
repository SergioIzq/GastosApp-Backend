namespace AhorroLand.Shared.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de hashing de contraseñas.
/// Encapsula la lógica de hashing seguro de contraseñas.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash seguro de una contraseña.
    /// </summary>
    /// <param name="password">La contraseña en texto plano.</param>
    /// <returns>El hash de la contraseña.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica si una contraseña coincide con el hash almacenado.
    /// </summary>
    /// <param name="password">La contraseña en texto plano a verificar.</param>
    /// <param name="hashedPassword">El hash almacenado.</param>
    /// <returns>True si la contraseña es válida, false en caso contrario.</returns>
    bool VerifyPassword(string password, string hashedPassword);
}
