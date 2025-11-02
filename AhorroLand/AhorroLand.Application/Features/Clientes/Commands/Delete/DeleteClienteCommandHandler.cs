using AhorroLand.Domain.Categorias;
using AhorroLand.Domain.Clientes;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Categorias.Commands.DeleteCategoria;

/// <summary>
/// Manejador concreto para eliminar un Cliente.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeleteClienteCommandHandler
    : DeleteCommandHandler<Cliente, DeleteClienteCommand>
{
    public DeleteClienteCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Cliente> writeRepository,
        IReadRepository<Cliente> readOnlyRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, readOnlyRepository, cacheService)
    {
    }
}