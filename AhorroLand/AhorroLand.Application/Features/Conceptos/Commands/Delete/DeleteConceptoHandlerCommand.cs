using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.Conceptos.Commands;

/// <summary>
/// Manejador concreto para eliminar un Concepto.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeleteConceptoCommandHandler
    : DeleteCommandHandler<Concepto, DeleteConceptoCommand>
{
    public DeleteConceptoCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<Concepto> writeRepository,
        IReadRepository<Concepto> readOnlyRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, readOnlyRepository, cacheService)
    {
    }
}