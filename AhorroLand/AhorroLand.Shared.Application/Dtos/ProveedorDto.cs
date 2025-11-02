namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación del proveedor para ser enviada fuera de la capa de aplicación.
    /// </summary>
    public record ProveedorDto(
        Guid Id,
        string Nombre
    );
}