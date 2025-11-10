using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.GastosProgramados.Commands;

public sealed class DeleteGastoProgramadoCommandHandler
    : DeleteCommandHandler<GastoProgramado, DeleteGastoProgramadoCommand>
{
    public DeleteGastoProgramadoCommandHandler(
     IUnitOfWork unitOfWork,
   IWriteRepository<GastoProgramado> writeRepository,
        IReadRepository<GastoProgramado> readOnlyRepository,
        ICacheService cacheService)
  : base(unitOfWork, writeRepository, readOnlyRepository, cacheService)
    {
    }
}
