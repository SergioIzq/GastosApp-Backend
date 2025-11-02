namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación de la persona para ser enviada fuera de la capa de aplicación.
    /// </summary>
    public record PersonaDto(
        Guid Id,
        string Nombre,
        Guid UsuarioId
    );
}