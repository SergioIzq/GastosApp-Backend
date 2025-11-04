using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Categorias.Commands;

/// <summary>
/// Representa la solicitud para actualizar una nueva Categoría.
/// </summary>
// Hereda de AbsUpadteCommand<Entidad, DTO de Respuesta>
public sealed record UpdateCategoriaCommand : AbsUpdateCommand<Categoria, CategoriaDto>
{
    /// <summary>
    /// Nombre de la nueva categoría.
    /// </summary>
    public required string Nombre { get; init; }

    /// <summary>
    /// Descripción opcional de la categoría.
    /// </summary>
    public string? Descripcion { get; init; }
}