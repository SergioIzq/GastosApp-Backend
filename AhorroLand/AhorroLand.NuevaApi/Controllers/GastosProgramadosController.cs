using AhorroLand.Application.Features.GastosProgramados.Commands;
using AhorroLand.Application.Features.GastosProgramados.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/gastos-programados")]
public class GastosProgramadosController : AbsController
{
    public GastosProgramadosController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetGastosProgramadosPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetGastoProgramadoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGastoProgramadoRequest request)
    {
        var command = new CreateGastoProgramadoCommand
        {
            Importe = request.Importe,
            Frecuencia = request.Frecuencia,
            FechaEjecucion = request.FechaEjecucion,
            Descripcion = request.Descripcion,
            ConceptoId = request.ConceptoId,
            ConceptoNombre = request.ConceptoNombre,
            ProveedorId = request.ProveedorId,
            CategoriaId = request.CategoriaId,
            PersonaId = request.PersonaId,
            CuentaId = request.CuentaId,
            FormaPagoId = request.FormaPagoId
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGastoProgramadoRequest request)
    {
        var command = new UpdateGastoProgramadoCommand
        {
            Id = id,
            Importe = request.Importe,
            Frecuencia = request.Frecuencia,
            FechaEjecucion = request.FechaEjecucion,
            Descripcion = request.Descripcion,
            ConceptoId = request.ConceptoId,
            ConceptoNombre = request.ConceptoNombre,
            ProveedorId = request.ProveedorId,
            CategoriaId = request.CategoriaId,
            PersonaId = request.PersonaId,
            CuentaId = request.CuentaId,
            FormaPagoId = request.FormaPagoId
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteGastoProgramadoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateGastoProgramadoRequest(
    decimal Importe,
    string Frecuencia,
    DateTime? FechaEjecucion,
    string? Descripcion,
    Guid ConceptoId,
    string ConceptoNombre,
    Guid ProveedorId,
    Guid CategoriaId,
    Guid PersonaId,
    Guid CuentaId,
    Guid FormaPagoId
);

public record UpdateGastoProgramadoRequest(
    decimal Importe,
    string Frecuencia,
    DateTime? FechaEjecucion,
    string? Descripcion,
    Guid ConceptoId,
    string ConceptoNombre,
    Guid ProveedorId,
    Guid CategoriaId,
    Guid PersonaId,
    Guid CuentaId,
    Guid FormaPagoId
);
