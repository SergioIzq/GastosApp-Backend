using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.TraspasosProgramados.Commands;

public sealed class DeleteTraspasoProgramadoCommandHandler
  : DeleteCommandHandler<TraspasoProgramado, DeleteTraspasoProgramadoCommand>
{
    public DeleteTraspasoProgramadoCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<TraspasoProgramado> writeRepository,
  IReadRepository<TraspasoProgramado> readOnlyRepository,
     ICacheService cacheService)
    : base(unitOfWork, writeRepository, readOnlyRepository, cacheService)
    {
    }
}
