using AhorroLand.Application.Features.Dashboard.Queries;
using AhorroLand.NuevaApi.Controllers.Base;
using AhorroLand.Shared.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AhorroLand.NuevaApi.Controllers;

/// <summary>
/// Controller para el dashboard con métricas y resumen del usuario.
/// </summary>
[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController : AbsController
{
    public DashboardController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Obtiene el resumen completo del dashboard para el usuario autenticado con filtros opcionales.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio del período (opcional, default: primer día del mes actual)</param>
    /// <param name="fechaFin">Fecha de fin del período (opcional, default: último día del mes actual)</param>
    /// <param name="cuentaId">Filtrar por cuenta específica (opcional)</param>
    /// <param name="categoriaId">Filtrar por categoría específica (opcional)</param>
    /// <returns>Resumen con balance, ingresos, gastos, top categorías, histórico, alertas y más.</returns>
    /// <response code="200">Resumen obtenido correctamente.</response>
    /// <response code="401">Usuario no autenticado.</response>
    /// <response code="404">No se encontraron datos para el usuario.</response>
    /// <response code="500">Error interno del servidor.</response>
    [HttpGet("resumen")]
    [ProducesResponseType(typeof(DashboardResumenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetResumen(
           [FromQuery] DateTime? fechaInicio = null,
    [FromQuery] DateTime? fechaFin = null,
      [FromQuery] Guid? cuentaId = null,
      [FromQuery] Guid? categoriaId = null)
    {
        // Obtener el UsuarioId del token JWT
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdClaim) || !Guid.TryParse(usuarioIdClaim, out var usuarioId))
        {
            return Unauthorized(new { message = "Token inválido o usuario no identificado." });
        }

        // Validar que fechaFin sea posterior a fechaInicio si ambas están presentes
        if (fechaInicio.HasValue && fechaFin.HasValue && fechaFin < fechaInicio)
        {
            return BadRequest(new { message = "La fecha de fin debe ser posterior a la fecha de inicio." });
        }

        var query = new GetDashboardResumenQuery(
       usuarioId,
       fechaInicio,
       fechaFin,
     cuentaId,
        categoriaId);

        var result = await _sender.Send(query);

        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene el histórico de los últimos N meses (sin filtros adicionales).
    /// </summary>
    /// <param name="meses">Número de meses a consultar (default: 6, max: 12)</param>
    /// <response code="200">Histórico obtenido correctamente.</response>
    [HttpGet("historico")]
    [ProducesResponseType(typeof(List<HistoricoMensualDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorico([FromQuery] int meses = 6)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdClaim) || !Guid.TryParse(usuarioIdClaim, out var usuarioId))
        {
            return Unauthorized(new { message = "Token inválido o usuario no identificado." });
        }

        // Validar límites
        if (meses < 1 || meses > 12)
        {
            return BadRequest(new { message = "El número de meses debe estar entre 1 y 12." });
        }

        var query = new GetDashboardResumenQuery(usuarioId);
        var result = await _sender.Send(query);

        if (result.IsSuccess)
        {
            // Filtrar solo el histórico de los N meses solicitados
            var historico = result.Value.HistoricoUltimos6Meses.Take(meses).ToList();
            return Ok(historico);
        }

        return HandleResult(result);
    }
}
