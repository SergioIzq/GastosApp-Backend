using AhorroLand.Application.Features.IngresosProgramados.Commands;
using AhorroLand.Application.Features.IngresosProgramados.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/ingresos-programados")]
public class IngresosProgramadosController : AbsController
{
    public IngresosProgramadosController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetIngresosProgramadosPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetIngresoProgramadoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIngresoProgramadoRequest request)
    {
        var command = new CreateIngresoProgramadoCommand
        {
            Importe = request.Importe,
            Frecuencia = request.Frecuencia,
            FechaEjecucion = request.FechaEjecucion,
            Descripcion = request.Descripcion,
            ConceptoId = request.ConceptoId,
            ConceptoNombre = request.ConceptoNombre,
            CategoriaId = request.CategoriaId,
            ClienteId = request.ClienteId,
            ClienteNombre = request.ClienteNombre,
            PersonaId = request.PersonaId,
            PersonaNombre = request.PersonaNombre,
            CuentaId = request.CuentaId,
            CuentaNombre = request.CuentaNombre,
            FormaPagoId = request.FormaPagoId,
            FormaPagoNombre = request.FormaPagoNombre
        };

        var result = await _sender.Send(command);

        return HandleResultForCreation(
           result,
        nameof(GetById),
    new { id = result.Value.Id }
          );
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIngresoProgramadoRequest request)
    {
        var command = new UpdateIngresoProgramadoCommand
        {
            Id = id,
            Importe = request.Importe,
            Frecuencia = request.Frecuencia,
            FechaEjecucion = request.FechaEjecucion,
            Descripcion = request.Descripcion,
            ConceptoId = request.ConceptoId,
            ConceptoNombre = request.ConceptoNombre,
            CategoriaId = request.CategoriaId,
            ClienteId = request.ClienteId,
            ClienteNombre = request.ClienteNombre,
            PersonaId = request.PersonaId,
            PersonaNombre = request.PersonaNombre,
            CuentaId = request.CuentaId,
            CuentaNombre = request.CuentaNombre,
            FormaPagoId = request.FormaPagoId,
            FormaPagoNombre = request.FormaPagoNombre
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteIngresoProgramadoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateIngresoProgramadoRequest(
    decimal Importe,
    string Frecuencia,
    DateTime FechaEjecucion,
    string? Descripcion,
    Guid ConceptoId,
    string ConceptoNombre,
    Guid CategoriaId,
    Guid ClienteId,
 string ClienteNombre,
  Guid PersonaId,
    string PersonaNombre,
    Guid CuentaId,
    string CuentaNombre,
    Guid FormaPagoId,
    string FormaPagoNombre
);

public record UpdateIngresoProgramadoRequest(
    decimal Importe,
    string Frecuencia,
DateTime FechaEjecucion,
 string? Descripcion,
    Guid ConceptoId,
    string ConceptoNombre,
    Guid CategoriaId,
    Guid ClienteId,
string ClienteNombre,
  Guid PersonaId,
    string PersonaNombre,
 Guid CuentaId,
    string CuentaNombre,
    Guid FormaPagoId,
    string FormaPagoNombre
);
