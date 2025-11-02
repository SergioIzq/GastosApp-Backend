using AhorroLand.Domain.Conceptos;
using AhorroLand.Domain.Cuentas;
using AhorroLand.Domain.FormasPago;
using AhorroLand.Domain.Gastos;
using AhorroLand.Domain.Personas;
using AhorroLand.Domain.Proveedores;
using AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Application.Dtos;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Gastos.Commands;

public sealed class CreateGastoCommandHandler : AbsCreateCommandHandler<Gasto, GastoDto, CreateGastoCommand>
{
 private readonly IReadOnlyRepository<Concepto> _conceptoRepo;
 private readonly IReadOnlyRepository<Cuenta> _cuentaRepo;
 private readonly IReadOnlyRepository<FormaPago> _formaPagoRepo;
 private readonly IReadOnlyRepository<Proveedor> _proveedorRepo;
 private readonly IReadOnlyRepository<Persona> _personaRepo;

 public CreateGastoCommandHandler(
 IUnitOfWork unitOfWork,
 IWriteRepository<Gasto> writeRepository,
 ICacheService cacheService,
 IReadOnlyRepository<Concepto> conceptoRepo,
 IReadOnlyRepository<Cuenta> cuentaRepo,
 IReadOnlyRepository<FormaPago> formaPagoRepo,
 IReadOnlyRepository<Proveedor> proveedorRepo,
 IReadOnlyRepository<Persona> personaRepo)
 : base(unitOfWork, writeRepository, cacheService)
 {
 _conceptoRepo = conceptoRepo;
 _cuentaRepo = cuentaRepo;
 _formaPagoRepo = formaPagoRepo;
 _proveedorRepo = proveedorRepo;
 _personaRepo = personaRepo;
 }

 protected override Gasto CreateEntity(CreateGastoCommand command)
 {
 var concepto = _conceptoRepo.GetByIdAsync(command.ConceptoId).ConfigureAwait(false).GetAwaiter().GetResult();
 var cuenta = _cuentaRepo.GetByIdAsync(command.CuentaId).ConfigureAwait(false).GetAwaiter().GetResult();
 var formaPago = _formaPagoRepo.GetByIdAsync(command.FormaPagoId).ConfigureAwait(false).GetAwaiter().GetResult();
 Proveedor? proveedor = null;
 Persona? persona = null;

 if (command.ProveedorId.HasValue)
 proveedor = _proveedorRepo.GetByIdAsync(command.ProveedorId.Value).ConfigureAwait(false).GetAwaiter().GetResult();
 if (command.PersonaId.HasValue)
 persona = _personaRepo.GetByIdAsync(command.PersonaId.Value).ConfigureAwait(false).GetAwaiter().GetResult();

 if (concepto is null || cuenta is null || formaPago is null)
 throw new InvalidOperationException("Concepto, Cuenta o FormaPago no encontrados.");

 var gasto = Gasto.Create(
 Guid.NewGuid(),
 command.Importe,
 command.Fecha,
 concepto,
 proveedor!,
 persona!,
 cuenta,
 formaPago,
 command.Descripcion);

 return gasto;
 }
}
