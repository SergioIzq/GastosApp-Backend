using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Personas.Commands;

/// <summary>
/// Representa la solicitud para actualizar una nueva cuenta.
/// </summary>
// Hereda de AbsUpadteCommand<Entidad, DTO de Respuesta>
public sealed record UpdatePersonaCommand : AbsUpdateCommand<Persona, PersonaDto>
{
    /// <summary>
    /// Nombre de la nueva cuenta.
    /// </summary>
    public required string Nombre { get; init; }
}