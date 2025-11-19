using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;

namespace AhorroLand.Application.Features.FormasPago.Commands;

/// <summary>
/// Manejador concreto para eliminar una FormaPago.
/// Hereda toda la lógica de la clase base genérica.
/// </summary>
public sealed class DeleteFormaPagoCommandHandler
    : DeleteCommandHandler<FormaPago, DeleteFormaPagoCommand>
{
    public DeleteFormaPagoCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<FormaPago> writeRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, cacheService)
    {
    }
}


