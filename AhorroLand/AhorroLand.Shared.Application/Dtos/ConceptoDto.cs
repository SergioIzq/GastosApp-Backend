namespace AhorroLand.Shared.Application.Dtos
{
    /// <summary>
    /// Representación del Concepto, incluyendo la información esencial de su categoría.
    /// </summary>
    public record ConceptoDto(
        Guid Id,
        string Nombre,
        Guid CategoriaId,
        string CategoriaNombre
    );
}