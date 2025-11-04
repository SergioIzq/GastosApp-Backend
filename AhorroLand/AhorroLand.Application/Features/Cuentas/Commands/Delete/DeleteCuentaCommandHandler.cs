using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Cuentas.Commands;

/// <summary>
/// Manejador concreto para eliminar una Cuenta.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeleteCuentaCommandHandler
    : DeleteCommandHandler<Cuenta, DeleteCuentaCommand>
{
    public DeleteCuentaCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Cuenta> writeRepository,
        IReadRepository<Cuenta> readOnlyRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, readOnlyRepository, cacheService)
    {
    }
}