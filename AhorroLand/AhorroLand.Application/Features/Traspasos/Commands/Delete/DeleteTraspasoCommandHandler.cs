using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Traspasos.Commands;

public sealed class DeleteTraspasoCommandHandler
    : DeleteCommandHandler<Traspaso, DeleteTraspasoCommand>
{
    public DeleteTraspasoCommandHandler(
        IUnitOfWork unitOfWork,
  IWriteRepository<Traspaso> writeRepository,
        ICacheService cacheService)
      : base(unitOfWork, writeRepository, cacheService)
    {
    }
}



