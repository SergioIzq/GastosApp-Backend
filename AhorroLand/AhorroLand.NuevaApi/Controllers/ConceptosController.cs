using AhorroLand.Application.Features.Conceptos.Commands;
using AhorroLand.Application.Features.Conceptos.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/conceptos")]
public class ConceptosController : AbsController
{
    public ConceptosController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetConceptosPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetConceptoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateConceptoRequest request)
    {
        var command = new CreateConceptoCommand
        {
            Nombre = request.Nombre,
            CategoriaId = request.CategoriaId,
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConceptoRequest request)
    {
        var command = new UpdateConceptoCommand
        {
            Id = id,
            Nombre = request.Nombre,
            CategoriaId = request.CategoriaId
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteConceptoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateConceptoRequest(
string Nombre,
    Guid CategoriaId,
    Guid UsuarioId
);

public record UpdateConceptoRequest(
    string Nombre,
  Guid CategoriaId
);
