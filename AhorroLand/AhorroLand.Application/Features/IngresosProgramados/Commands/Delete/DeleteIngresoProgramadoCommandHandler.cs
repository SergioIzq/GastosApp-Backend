using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.IngresosProgramados.Commands;

public sealed class DeleteIngresoProgramadoCommandHandler
    : DeleteCommandHandler<IngresoProgramado, DeleteIngresoProgramadoCommand>
{
    public DeleteIngresoProgramadoCommandHandler(
      IUnitOfWork unitOfWork,
        IWriteRepository<IngresoProgramado> writeRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, cacheService)
    {
    }
}



