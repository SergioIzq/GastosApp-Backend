namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación del cliente para ser enviada fuera de la capa de aplicación.
    /// </summary>
    public record ClienteDto(
        Guid Id,
        string Nombre,
        Guid UsuarioId
    );
}