using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Proveedores.Commands;

/// <summary>
/// Manejador concreto para eliminar una Proveedor.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeleteProveedorCommandHandler
    : DeleteCommandHandler<Proveedor, DeleteProveedorCommand>
{
    public DeleteProveedorCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Proveedor> writeRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, cacheService)
    {
    }
}


