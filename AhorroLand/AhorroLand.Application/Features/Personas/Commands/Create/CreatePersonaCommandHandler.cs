using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Personas.Commands;

public sealed class CreatePersonaCommandHandler : AbsCreateCommandHandler<Persona, PersonaDto, CreatePersonaCommand>
{
    public CreatePersonaCommandHandler(
    IUnitOfWork unitOfWork,
    IWriteRepository<Persona> writeRepository,
    ICacheService cacheService)
    : base(unitOfWork, writeRepository, cacheService)
    {
    }

    protected override Persona CreateEntity(CreatePersonaCommand command)
    {
        var nombreVO = new Nombre(command.Nombre);
        var usuarioId = new UsuarioId(command.UsuarioId);

        var newPersona = Persona.Create(Guid.NewGuid(), nombreVO, usuarioId);
        return newPersona;
    }
}
