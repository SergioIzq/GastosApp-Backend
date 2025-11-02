using AhorroLand.Domain.FormasPago;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.FormasPago.Commands;

public sealed class CreateFormaPagoCommandHandler : AbsCreateCommandHandler<FormaPago, FormaPagoDto, CreateFormaPagoCommand>
{
    public CreateFormaPagoCommandHandler(
    IUnitOfWork unitOfWork,
    IWriteRepository<FormaPago> writeRepository,
    ICacheService cacheService)
    : base(unitOfWork, writeRepository, cacheService)
    {
    }

    protected override FormaPago CreateEntity(CreateFormaPagoCommand command)
    {
        var nombreVO = new Nombre(command.Nombre);
        var newFormaPago = FormaPago.Create(Guid.NewGuid(), nombreVO);
        return newFormaPago;
    }
}
