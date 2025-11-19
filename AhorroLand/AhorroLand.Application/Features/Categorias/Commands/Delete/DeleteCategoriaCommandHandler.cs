using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Categorias.Commands;

/// <summary>
/// Manejador concreto para eliminar una Categoría.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeleteCategoriaCommandHandler
    : DeleteCommandHandler<Categoria, DeleteCategoriaCommand>
{
    public DeleteCategoriaCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Categoria> writeRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, cacheService)
    {
    }
}


