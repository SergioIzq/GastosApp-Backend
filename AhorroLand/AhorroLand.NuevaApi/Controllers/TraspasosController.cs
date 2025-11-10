using AhorroLand.Application.Features.Traspasos.Commands;
using AhorroLand.Application.Features.Traspasos.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/traspasos")]
public class TraspasosController : AbsController
{
    public TraspasosController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetTraspasosPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetTraspasoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTraspasoRequest request)
    {
        var command = new CreateTraspasoCommand
        {
            CuentaOrigenId = request.CuentaOrigenId,
            CuentaDestinoId = request.CuentaDestinoId,
            UsuarioId = request.UsuarioId,
            Importe = request.Importe,
            Fecha = request.Fecha,
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTraspasoRequest request)
    {
        var command = new UpdateTraspasoCommand
        {
            Id = id,
            CuentaOrigenId = request.CuentaOrigenId,
            CuentaDestinoId = request.CuentaDestinoId,
            Importe = request.Importe,
            Fecha = request.Fecha,
            Descripcion = request.Descripcion
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteTraspasoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateTraspasoRequest(
    Guid CuentaOrigenId,
    Guid CuentaDestinoId,
    Guid UsuarioId,
    decimal Importe,
    DateTime Fecha,
    string? Descripcion
);

public record UpdateTraspasoRequest(
    Guid CuentaOrigenId,
    Guid CuentaDestinoId,
    decimal Importe,
    DateTime Fecha,
    string? Descripcion
);
