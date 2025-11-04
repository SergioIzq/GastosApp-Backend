using AhorroLand.Domain;
using AhorroLand.Domain.Traspasos.Eventos;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;
using Mapster;

namespace AhorroLand.Application.Features.Traspasos.Commands;

public sealed class CreateTraspasoCommandHandler : AbsCreateCommandHandler<Traspaso, TraspasoDto, CreateTraspasoCommand>
{
    private readonly IDomainValidator _validator;

    public CreateTraspasoCommandHandler(
    IUnitOfWork unitOfWork,
    IWriteRepository<Traspaso> writeRepository,
    ICacheService cacheService,
    IDomainValidator validator)
    : base(unitOfWork, writeRepository, cacheService)
    {
        _validator = validator;
    }

    public override async Task<Result<TraspasoDto>> Handle(
        CreateTraspasoCommand command, CancellationToken cancellationToken)
    {
        // 1. VALIDACIÓN EN PARALELO de existencia (SELECT 1)
        var validationTasks = new[]
        {
            _validator.ExistsAsync<Cuenta>(command.CuentaOrigenId),
            _validator.ExistsAsync<Cuenta>(command.CuentaDestinoId),
        };

        // Espera de forma asíncrona y eficiente
        var results = await Task.WhenAll(validationTasks);
        // ⭐ OPTIMIZACIÓN: results ahora es un array de bool (bool[]), eliminando GetAwaiter().GetResult()

        // 2. CHEQUEO DE ERRORES DE EXISTENCIA
        // results[0] es la existencia de CuentaOrigen, results[1] es CuentaDestino
        if (!results[0] || !results[1])
        {
            return Result.Failure<TraspasoDto>(
                Error.NotFound("Cuenta origen o destino no encontrada."));
        }

        // 3. VALIDACIÓN DE DOMINIO INTRÍNSECA
        if (command.CuentaOrigenId == command.CuentaDestinoId)
        {
            return Result.Failure<TraspasoDto>(
                Error.Validation("La cuenta origen y destino no pueden ser la misma."));
        }

        // 4. CREACIÓN DE VALUE OBJECTS y la ENTIDAD
        try
        {
            // Creamos VOs de valor
            var importeVO = new Cantidad(command.Importe);
            var fechaVO = new FechaRegistro(command.Fecha);
            var descripcionVO = new Descripcion(command.Descripcion ?? string.Empty);
            var usuarioIdVO = new UsuarioId(command.UsuarioId);

            // Creamos VOs de identidad
            var cuentaOrigenId = new CuentaId(command.CuentaOrigenId);
            var cuentaDestinoId = new CuentaId(command.CuentaDestinoId);

            // Creación de la Entidad (solo con VOs de identidad y valor)
            var traspaso = Traspaso.Create(cuentaOrigenId, cuentaDestinoId, importeVO, fechaVO, usuarioIdVO, descripcionVO);

            // 5. PERSISTENCIA
            _writeRepository.Add(traspaso);
            var entityResult = await CreateAsync(traspaso, cancellationToken);

            if (entityResult.IsFailure)
            {
                return Result.Failure<TraspasoDto>(entityResult.Error);
            }

            // 6. MAPEO Y ÉXITO
            var dto = entityResult.Value.Adapt<TraspasoDto>();

            traspaso.RaiseDomainEvent(new TraspasoRegistradoDomainEvent(traspaso.Id, cuentaOrigenId.Value, cuentaDestinoId.Value, importeVO));

            return Result.Success(dto);
        }
        catch (ArgumentException ex)
        {
            // Captura de errores de validación de Value Objects (ej: Importe <= 0)
            return Result.Failure<TraspasoDto>(Error.Validation(ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<TraspasoDto>(Error.Failure("Error.Unexpected", "Error Inesperado", ex.Message));
        }
    }

    // ⭐ Asegurar que el método síncrono no se usa
    protected override Traspaso CreateEntity(CreateTraspasoCommand command)
    {
        throw new NotImplementedException("CreateEntity no debe usarse. La lógica de creación asíncrona reside en el método Handle.");
    }

}