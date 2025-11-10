using AhorroLand.Application.Features.Ingresos.Commands;
using AhorroLand.Application.Features.Ingresos.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/ingresos")]
public class IngresosController : AbsController
{
    public IngresosController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetIngresosPagedListQuery(page, pageSize);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetIngresoByIdQuery(id);
        var result = await _sender.Send(query);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIngresoRequest request)
    {
        var command = new CreateIngresoCommand
        {
            Importe = request.Importe,
            Fecha = request.Fecha,
            Descripcion = request.Descripcion,
            CategoriaId = request.CategoriaId,
            CategoriaNombre = request.CategoriaNombre,
            ConceptoId = request.ConceptoId,
            ConceptoNombre = request.ConceptoNombre,
            ClienteId = request.ClienteId,
            ClienteNombre = request.ClienteNombre,
            PersonaId = request.PersonaId,
            PersonaNombre = request.PersonaNombre,
            CuentaId = request.CuentaId,
            CuentaNombre = request.CuentaNombre,
            FormaPagoId = request.FormaPagoId,
            FormaPagoNombre = request.FormaPagoNombre,
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIngresoRequest request)
    {
        var command = new UpdateIngresoCommand
        {
            Id = id,
            Importe = request.Importe,
            Fecha = request.Fecha,
            Descripcion = request.Descripcion,
            CategoriaId = request.CategoriaId,
            ConceptoId = request.ConceptoId,
            ClienteId = request.ClienteId,
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
        var command = new DeleteIngresoCommand(id);
        var result = await _sender.Send(command);
        return HandleResult(result);
    }
}

public record CreateIngresoRequest(
 decimal Importe,
 DateTime Fecha,
    string? Descripcion,
    Guid CategoriaId,
    string CategoriaNombre,
    Guid ConceptoId,
    string ConceptoNombre,
    Guid ClienteId,
  string ClienteNombre,
    Guid PersonaId,
 string PersonaNombre,
    Guid CuentaId,
    string CuentaNombre,
    Guid FormaPagoId,
    string FormaPagoNombre,
    Guid UsuarioId
);

public record UpdateIngresoRequest(
decimal Importe,
    DateTime Fecha,
    string? Descripcion,
    Guid CategoriaId,
    Guid ConceptoId,
    Guid ClienteId,
    Guid PersonaId,
    Guid CuentaId,
    Guid FormaPagoId,
  Guid UsuarioId
);
