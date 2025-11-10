using AhorroLand.Application.Features.Personas.Commands;
using AhorroLand.Application.Features.Personas.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/personas")]
public class PersonasController : AbsController
{
    public PersonasController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetPersonasPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetPersonaByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonaRequest request)
    {
        var command = new CreatePersonaCommand
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonaRequest request)
    {
        var command = new UpdatePersonaCommand
        {
            Id = id,
            Nombre = request.Nombre
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeletePersonaCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreatePersonaRequest(
    string Nombre,
 Guid UsuarioId
);

public record UpdatePersonaRequest(
    string Nombre
);
