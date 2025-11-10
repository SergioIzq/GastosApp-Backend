using AhorroLand.Application.Features.Categorias.Commands;
using AhorroLand.Application.Features.Categorias.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/categorias")]
public class CategoriasController : AbsController
{
    public CategoriasController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetCategoriasPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetCategoriaByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaRequest request)
    {
        var command = new CreateCategoriaCommand
        {
            Nombre = request.Nombre,
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoriaRequest request)
    {
        var command = new UpdateCategoriaCommand
        {
            Id = id,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCategoriaCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateCategoriaRequest(
    string Nombre,
    Guid UsuarioId,
    string? Descripcion
);

public record UpdateCategoriaRequest(
    string Nombre,
    string? Descripcion
);
