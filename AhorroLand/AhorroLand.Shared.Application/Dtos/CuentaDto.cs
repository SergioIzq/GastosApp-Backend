namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación de la Cuenta para ser enviada fuera de la capa de aplicación.
    /// </summary>
    public record CuentaDto(
        Guid Id,
        string Nombre,
        decimal Saldo
    );
}