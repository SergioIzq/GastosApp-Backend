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
        Guid? CategoriaId, // 🔥 NULLABLE: CategoriaId viene del Concepto (LEFT JOIN)
        string? CategoriaNombre, // 🔥 NULLABLE: puede ser null si no hay categoría

        Guid? ProveedorId, // 🔥 NULLABLE: el gasto puede no tener proveedor
        string? ProveedorNombre, // 🔥 NULLABLE

        Guid? PersonaId, // 🔥 NULLABLE: el gasto puede no tener persona
        string? PersonaNombre, // 🔥 NULLABLE

        Guid CuentaId,
        string CuentaNombre,

        Guid FormaPagoId,
        string FormaPagoNombre,

        Guid UsuarioId
    );
}