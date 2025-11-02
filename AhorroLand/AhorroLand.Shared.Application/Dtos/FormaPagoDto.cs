namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación de la FormaPago para ser enviada fuera de la capa de aplicación.
    /// </summary>
    public record FormaPagoDto(
        Guid Id,
        string Nombre
    );
}