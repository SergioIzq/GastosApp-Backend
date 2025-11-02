namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación del Gasto para ser expuesta. Contiene IDs y datos clave de las entidades relacionadas.
    /// </summary>
    public record GastoDto(
        Guid Id,
        decimal Importe,
        DateTime Fecha,
        string? Descripcion,

        // Relaciones (Flattened)
        Guid ConceptoId,
        string ConceptoNombre,
        Guid CategoriaId,
        string CategoriaNombre,

        Guid ProveedorId,
        string ProveedorNombre,

        Guid PersonaId,
        string PersonaNombre,

        Guid CuentaId,
        string CuentaNombre,

        Guid FormaPagoId,
        string FormaPagoNombre,

        Guid UsuarioId
    );
}