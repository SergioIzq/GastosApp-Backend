using AhorroLand.Application.Features.Clientes.Commands;
using AhorroLand.Application.Features.Clientes.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Route("api/clientes")]
public class ClientesController : AbsController
{
    // Inyectas ISender (Mediator)
    public ClientesController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        // 1. Creas un Query (objeto de la capa Application)
        var query = new GetClienteByIdQuery(id);

        // 2. Lo envías con Mediator
        var result = await _sender.Send(query); // Devuelve Result<ClienteDto>

        // 3. Dejas que el ApiBaseController lo traduzca
        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest request)
    {
        // 1. Mapeas el Request (DTO de API) a un Command (de Application)
        var command = new CreateClienteCommand(
            request.Nombre,
            request.UsuarioId
        );

        // 2. Lo envías
        var result = await _sender.Send(command); // Devuelve Result<ClienteDto>

        // 3. Usas el handler de creación (devuelve 201 Created)
        return HandleResultForCreation(
            result,
            nameof(GetById), // Nombre de la acción para 'GetById'
            new { id = result.Value.Id } // Parámetros de ruta
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteClienteCommand(id);

        var result = await _sender.Send(command);

        return HandleResult(result);
    }

}

// Este es el DTO que recibe la API, NO la entidad de dominio
public record CreateClienteRequest(
    string Nombre,
    Guid UsuarioId
);
