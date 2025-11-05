using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Personas.Queries;

/// <summary>
/// Maneja la creación de una nueva entidad Persona.
/// </summary>
public sealed class GetPersonaByIdQueryHandler
    : GetByIdQueryHandler<Persona, PersonaDto, GetPersonaByIdQuery>
{
    public GetPersonaByIdQueryHandler(
        ICacheService cacheService,
        IReadRepository<Persona> readOnlyRepository
        )
        : base(readOnlyRepository, cacheService)
    {
    }
}