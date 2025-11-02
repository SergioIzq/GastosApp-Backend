using AhorroLand.Domain.Categorias;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Categorias.Commands;

/// <summary>
/// Representa la solicitud para crear una nueva Categoría.
/// </summary>
// Hereda de AbsCreateCommand<Entidad, DTO de Respuesta>
public sealed record CreateCategoriaCommand : AbsCreateCommand<Categoria, CategoriaDto>
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