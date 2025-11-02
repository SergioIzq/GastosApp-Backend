using AhorroLand.Domain.Cuentas;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Cuentas.Commands;

public sealed class CreateCuentaCommandHandler : AbsCreateCommandHandler<Cuenta, CuentaDto, CreateCuentaCommand>
{
    public CreateCuentaCommandHandler(
    IUnitOfWork unitOfWork,
    IWriteRepository<Cuenta> writeRepository,
    ICacheService cacheService)
    : base(unitOfWork, writeRepository, cacheService)
    {
    }

    protected override Cuenta CreateEntity(CreateCuentaCommand command)
    {
        var nombreVO = new Nombre(command.Nombre);
        var saldoVO = new Cantidad(command.Saldo);
        var newCuenta = Cuenta.Create(Guid.NewGuid(), nombreVO, saldoVO);
        return newCuenta;
    }
}
