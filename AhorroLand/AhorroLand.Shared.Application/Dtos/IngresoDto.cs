namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación del Ingreso para ser expuesta. Contiene IDs y datos clave de las entidades relacionadas.
    /// </summary>
    public record IngresoDto(
        Guid Id,
        decimal Importe,
        DateTime Fecha,
        string? Descripcion,

        // Relaciones (Flattened)
        Guid ConceptoId,
        string ConceptoNombre,
        Guid CategoriaId,
        string CategoriaNombre,

        Guid ClienteId,
        string ClienteNombre,

        Guid PersonaId,
        string PersonaNombre,

        Guid CuentaId,
        string CuentaNombre,

        Guid FormaPagoId,
        string FormaPagoNombre,
        Guid UsuarioId
    );
}