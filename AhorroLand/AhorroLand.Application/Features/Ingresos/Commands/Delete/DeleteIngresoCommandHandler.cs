using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Ingresos.Commands;

/// <summary>
/// Manejador concreto para eliminar una Ingreso.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeleteIngresoCommandHandler
    : DeleteCommandHandler<Ingreso, DeleteIngresoCommand>
{
    public DeleteIngresoCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Ingreso> writeRepository,
        IReadRepository<Ingreso> readOnlyRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, readOnlyRepository, cacheService)
    {
    }
}