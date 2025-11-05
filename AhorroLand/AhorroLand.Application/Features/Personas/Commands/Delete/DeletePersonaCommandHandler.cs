using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Personas.Commands;

/// <summary>
/// Manejador concreto para eliminar una Persona.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeletePersonaCommandHandler
    : DeleteCommandHandler<Persona, DeletePersonaCommand>
{
    public DeletePersonaCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Persona> writeRepository,
        IReadRepository<Persona> readOnlyRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, readOnlyRepository, cacheService)
    {
    }
}