using AhorroLand.Application.Features.FormasPago.Commands;
using AhorroLand.Application.Features.FormasPago.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/formas-pago")]
public class FormasPagoController : AbsController
{
    public FormasPagoController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetFormasPagoPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetFormaPagoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFormaPagoRequest request)
    {
        var command = new CreateFormaPagoCommand
        {
            Nombre = request.Nombre,
            UsuarioId = request.UsuarioId
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFormaPagoRequest request)
    {
        var command = new UpdateFormaPagoCommand
        {
            Id = id,
            Nombre = request.Nombre
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteFormaPagoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateFormaPagoRequest(
    string Nombre,
Guid UsuarioId
);

public record UpdateFormaPagoRequest(
  string Nombre
);
