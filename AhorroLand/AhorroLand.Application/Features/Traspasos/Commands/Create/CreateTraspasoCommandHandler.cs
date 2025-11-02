using AhorroLand.Domain.Cuentas;
using AhorroLand.Domain.Traspasos;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Traspasos.Commands;

public sealed class CreateTraspasoCommandHandler : AbsCreateCommandHandler<Traspaso, TraspasoDto, CreateTraspasoCommand>
{
    private readonly IReadOnlyRepository<Cuenta> _cuentaRepository;

    public CreateTraspasoCommandHandler(
    IUnitOfWork unitOfWork,
    IWriteRepository<Traspaso> writeRepository,
    ICacheService cacheService,
    IReadOnlyRepository<Cuenta> cuentaRepository)
    : base(unitOfWork, writeRepository, cacheService)
    {
        _cuentaRepository = cuentaRepository;
    }

    protected override Traspaso CreateEntity(CreateTraspasoCommand command)
    {
        var cuentaOrigen = _cuentaRepository.GetByIdAsync(command.CuentaOrigenId).ConfigureAwait(false).GetAwaiter().GetResult();
        var cuentaDestino = _cuentaRepository.GetByIdAsync(command.CuentaDestinoId).ConfigureAwait(false).GetAwaiter().GetResult();

        if (cuentaOrigen is null || cuentaDestino is null)
            throw new InvalidOperationException("Cuenta origen o destino no encontrada.");

        var traspaso = Traspaso.Create(cuentaOrigen, cuentaDestino, command.Importe, command.Fecha, command.Descripcion);
        return traspaso;
    }
}
