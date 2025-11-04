using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Clientes.Commands;

/// <summary>
/// Representa la solicitud para actualizar una nueva Categoría.
/// </summary>
// Hereda de AbsUpadteCommand<Entidad, DTO de Respuesta>
public sealed record UpdateClienteCommand : AbsUpdateCommand<Cliente, ClienteDto>
{
    /// <summary>
    /// Nombre de la nueva categoría.
    /// </summary>
    public required string Nombre { get; init; }
}