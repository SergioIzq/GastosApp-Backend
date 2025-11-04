using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Dtos;

namespace AhorroLand.Application.Features.Personas.Commands;

public sealed record CreatePersonaCommand : AbsCreateCommand<Persona, PersonaDto>
{
    public required string Nombre { get; init; }
    public required Guid UsuarioId { get; init; }
}
