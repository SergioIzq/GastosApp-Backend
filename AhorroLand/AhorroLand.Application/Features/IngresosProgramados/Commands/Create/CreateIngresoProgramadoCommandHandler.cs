using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.IngresosProgramados.Commands;

public sealed class CreateIngresoProgramadoCommandHandler
    : AbsCreateCommandHandler<IngresoProgramado, IngresoProgramadoDto, CreateIngresoProgramadoCommand>
{
    private readonly IJobSchedulingService _jobSchedulingService;

    public CreateIngresoProgramadoCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<IngresoProgramado> writeRepository,
        ICacheService cacheService,
        IJobSchedulingService jobSchedulingService)
    : base(unitOfWork, writeRepository, cacheService)
    {
        _jobSchedulingService = jobSchedulingService;
    }

    protected override IngresoProgramado CreateEntity(CreateIngresoProgramadoCommand command)
    {
        var importeVO = new Cantidad(command.Importe);
        var frecuenciaVO = new Frecuencia(command.Frecuencia);
        var descripcionVO = new Descripcion(command.Descripcion);
        var conceptoIdVO = new ConceptoId(command.ConceptoId);
        var categoriaIdVO = new CategoriaId(command.CategoriaId);
        var clienteIdVO = new ClienteId(command.ClienteId);
        var personaIdVO = new PersonaId(command.PersonaId);
        var cuentaIdVO = new CuentaId(command.CuentaId);
        var formaPagoIdVO = new FormaPagoId(command.FormaPagoId);

        // Uso del servicio de infraestructura para generar el JobId
        var hangfireJobId = _jobSchedulingService.GenerateJobId();

        var newIngresoProgramado = IngresoProgramado.Create(
            importeVO,
            command.FechaEjecucion,
            conceptoIdVO,
            categoriaIdVO,
            clienteIdVO,
            frecuenciaVO,
            personaIdVO,
            cuentaIdVO,
            formaPagoIdVO,
            hangfireJobId,
            descripcionVO
        );

        return newIngresoProgramado;
    }
}

