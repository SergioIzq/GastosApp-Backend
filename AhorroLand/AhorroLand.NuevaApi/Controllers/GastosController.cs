using AhorroLand.Application.Features.Gastos.Commands;
using AhorroLand.Application.Features.Gastos.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/gastos")]
public class GastosController : AbsController
{
    public GastosController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetGastosPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetGastoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGastoRequest request)
    {
        var command = new CreateGastoCommand
        {
            Importe = request.Importe,
            Fecha = request.Fecha,
            Descripcion = request.Descripcion,
            CategoriaId = request.CategoriaId,
            ConceptoId = request.ConceptoId,
            ProveedorId = request.ProveedorId,
            PersonaId = request.PersonaId,
            CuentaId = request.CuentaId,
            FormaPagoId = request.FormaPagoId,
            UsuarioId = request.UsuarioId
        };

        var result = await _sender.Send(command);

        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGastoRequest request)
    {
        var command = new UpdateGastoCommand
        {
            Id = id,
            Importe = request.Importe,
            Fecha = request.Fecha,
            Descripcion = request.Descripcion,
            CategoriaId = request.CategoriaId,
            ConceptoId = request.ConceptoId,
            ProveedorId = request.ProveedorId,
            PersonaId = request.PersonaId,
            CuentaId = request.CuentaId,
            FormaPagoId = request.FormaPagoId,
            UsuarioId = request.UsuarioId
        };

        var result = await _sender.Send(command);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteGastoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateGastoRequest(
    decimal Importe,
    DateTime Fecha,
    string? Descripcion,
    Guid CategoriaId,
    Guid ConceptoId,
    Guid ProveedorId,
    Guid PersonaId,
    Guid CuentaId,
    Guid FormaPagoId,
    Guid UsuarioId
);

public record UpdateGastoRequest(
    decimal Importe,
    DateTime Fecha,
    string? Descripcion,
    Guid CategoriaId,
    Guid ConceptoId,
    Guid ProveedorId,
    Guid PersonaId,
    Guid CuentaId,
    Guid FormaPagoId,
    Guid UsuarioId
);
