using AhorroLand.Application.Features.TraspasosProgramados.Commands;
using AhorroLand.Application.Features.TraspasosProgramados.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/traspasos-programados")]
public class TraspasosProgramadosController : AbsController
{
    public TraspasosProgramadosController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetTraspasosProgramadosPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetTraspasoProgramadoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTraspasoProgramadoRequest request)
    {
        var command = new CreateTraspasoProgramadoCommand
        {
            CuentaOrigenId = request.CuentaOrigenId,
            CuentaDestinoId = request.CuentaDestinoId,
            Importe = request.Importe,
            FechaEjecucion = request.FechaEjecucion,
            Frecuencia = request.Frecuencia,
            UsuarioId = request.UsuarioId,
            Descripcion = request.Descripcion
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTraspasoProgramadoRequest request)
    {
        var command = new UpdateTraspasoProgramadoCommand
        {
            Id = id,
            CuentaOrigenId = request.CuentaOrigenId,
            CuentaDestinoId = request.CuentaDestinoId,
            Importe = request.Importe,
            FechaEjecucion = request.FechaEjecucion,
            Frecuencia = request.Frecuencia,
            UsuarioId = request.UsuarioId,
            HangfireJobId = request.HangfireJobId,
            Descripcion = request.Descripcion
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteTraspasoProgramadoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateTraspasoProgramadoRequest(
    Guid CuentaOrigenId,
    Guid CuentaDestinoId,
    decimal Importe,
    DateTime FechaEjecucion,
    string Frecuencia,
    Guid UsuarioId,
    string? Descripcion
);

public record UpdateTraspasoProgramadoRequest(
    Guid CuentaOrigenId,
    Guid CuentaDestinoId,
    decimal Importe,
    DateTime FechaEjecucion,
    string Frecuencia,
    Guid UsuarioId,
    string HangfireJobId,
    string? Descripcion
);
