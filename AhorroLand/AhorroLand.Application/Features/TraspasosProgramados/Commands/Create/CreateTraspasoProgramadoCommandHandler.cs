using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;
using Mapster;

namespace AhorroLand.Application.Features.TraspasosProgramados.Commands;

public sealed class CreateTraspasoProgramadoCommandHandler
    : AbsCreateCommandHandler<TraspasoProgramado, TraspasoProgramadoDto, CreateTraspasoProgramadoCommand>
{
    private readonly IDomainValidator _validator;

    public CreateTraspasoProgramadoCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<TraspasoProgramado> writeRepository,
        ICacheService cacheService,
        IDomainValidator validator)
    : base(unitOfWork, writeRepository, cacheService)
    {
        _validator = validator;
    }

    public override async Task<Result<TraspasoProgramadoDto>> Handle(
        CreateTraspasoProgramadoCommand command, CancellationToken cancellationToken)
    {
        // 1. VALIDACIÓN ASÍNCRONA EN PARALELO (Máxima Optimización I/O)
        var validationTasks = new[]
        {
            // Validaciones obligatorias
            _validator.ExistsAsync<Cuenta>(command.CuentaOrigenId),
            _validator.ExistsAsync<Cuenta>(command.CuentaDestinoId),
        };

        // Espera a que todas las consultas terminen al mismo tiempo.
        var results = await Task.WhenAll(validationTasks);

        // 2. CHEQUEO RÁPIDO DE ERRORES DE EXISTENCIA
        if (results.Any(r => !r))
        {
            // Retorno de error con el mensaje de Error.NotFound
            return Result.Failure<TraspasoProgramadoDto>(
                Error.NotFound("Una o más entidades referenciadas (CuentaOrigen, CuentaDestino) no existen."));
        }

        // 3. CREACIÓN DE VALUE OBJECTS (VOs) DENTRO DE UN TRY-CATCH
        // Volvemos al try-catch para manejar las ArgumentException lanzadas por los VOs.
        try
        {
            // Creación de VOs, que ahora son los que lanzan ArgumentException
            var importe = new Cantidad(command.Importe);
            var frecuencia = new Frecuencia(command.Frecuencia);
            var descripcion = new Descripcion(command.Descripcion ?? string.Empty);

            // Creamos VOs de Identidad
            var cuentaOrigenId = new CuentaId(command.CuentaOrigenId);
            var cuentaDestinoId = new CuentaId(command.CuentaDestinoId);
            var usuarioId = new UsuarioId(command.UsuarioId);

            // 4. CREACIÓN DE LA ENTIDAD DE DOMINIO (TraspasoProgramado)
            var traspasoProgramadoResult = TraspasoProgramado.Create(
                cuentaOrigenId,
                cuentaDestinoId,
                importe,
                command.FechaEjecucion,
                frecuencia,
                usuarioId,
                command.HangfireJobId,
                descripcion
            );

            if (traspasoProgramadoResult.IsFailure)
            {
                return Result.Failure<TraspasoProgramadoDto>(traspasoProgramadoResult.Error);
            }

            // 5. PERSISTENCIA
            _writeRepository.Add(traspasoProgramadoResult.Value);
            var entityResult = await base.CreateAsync(traspasoProgramadoResult.Value, cancellationToken);

            if (entityResult.IsFailure)
            {
                return Result.Failure<TraspasoProgramadoDto>(entityResult.Error);
            }

            // 6. MAPEO Y ÉXITO
            var dto = traspasoProgramadoResult.Value.Adapt<TraspasoProgramadoDto>();

            return Result.Success(dto);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<TraspasoProgramadoDto>(Error.Validation(ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<TraspasoProgramadoDto>(Error.Failure("Error.Unexpected", "Error Inesperado", ex.Message));
        }
    }

    protected override TraspasoProgramado CreateEntity(CreateTraspasoProgramadoCommand command)
    {
        throw new NotImplementedException("CreateEntity no debe usarse. La lógica de creación asíncrona reside en el método Handle.");
    }
}
