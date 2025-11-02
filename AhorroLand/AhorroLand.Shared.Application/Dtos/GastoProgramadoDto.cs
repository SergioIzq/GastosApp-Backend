namespace AhorroLand.Shared.Application.Dtos
{
    public record GastoProgramadoDto(
        Guid Id,
        decimal Importe,
        DateTime FechaEjecucion,
        string? Descripcion,

        // ⭐ PROPIEDADES FALTANTES AÑADIDAS
        string Frecuencia,
        bool Activo,
        string HangfireJobId,

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