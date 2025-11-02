namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación de los datos públicos del Usuario para ser expuesta.
    /// NO incluye contraseñas, hashes, ni tokens de confirmación.
    /// </summary>
    public record UsuarioDto(
        Guid Id,
        string Correo,
        bool Activo
    );
}