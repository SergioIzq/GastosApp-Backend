using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.TraspasosProgramados.Queries;

public sealed class GetTraspasoProgramadoByIdQueryHandler
    : GetByIdQueryHandler<TraspasoProgramado, TraspasoProgramadoDto, GetTraspasoProgramadoByIdQuery>
{
    public GetTraspasoProgramadoByIdQueryHandler(
    ICacheService cacheService,
           IReadRepository<TraspasoProgramado> readOnlyRepository
           )
     : base(readOnlyRepository, cacheService)
    {
    }
}
