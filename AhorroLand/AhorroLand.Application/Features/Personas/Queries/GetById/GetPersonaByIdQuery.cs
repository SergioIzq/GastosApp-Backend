using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Personas.Queries;

/// <summary>
/// Representa la solicitud para crear un nuevo Persona.
/// </summary>
// Hereda de AbsCreateCommand<Entidad, DTO de Respuesta>
public sealed record GetPersonaByIdQuery(Guid Id) : AbsGetByIdQuery<Persona, PersonaDto>(Id)
{
}