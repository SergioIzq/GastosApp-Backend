namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación de un Traspaso para ser expuesta. Contiene los IDs de las cuentas y los valores primitivos.
    /// </summary>
    public record TraspasoDto(
        Guid Id,

        // Value Objects
        decimal Importe,
        DateTime Fecha,
        string? Descripcion,

        // Relaciones (Flattened)
        Guid CuentaOrigenId,
        string CuentaOrigenNombre,
        Guid CuentaDestinoId,
        string CuentaDestinoNombre
    );
}