using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using AppG.BBDD;
using Microsoft.AspNetCore.Authorization;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/resumen")]
    [Authorize]
    public class ResumenController : BaseController<Resumen>
    {
        private readonly IResumenServicio _resumenService;

        public ResumenController(IResumenServicio resumenService) : base(resumenService)
        {
            _resumenService = resumenService;
        }

        [HttpGet("getCantidadIngresos")]
        public async Task<IActionResult> GetIngresosAsync(int page, int size, string periodoInicio, string periodoFin, int idUsuario)
        {
            var result = await _resumenService.GetIngresosAsync(page, size, periodoInicio, periodoFin, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpGet("getCantidadGastos")]
        public async Task<IActionResult> GetGastosAsync(int page, int size, string periodoInicio, string periodoFin, int idUsuario)
        {
            var result = await _resumenService.GetGastosAsync(page, size, periodoInicio, periodoFin, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel(ExportExcelRequest request)
        {

            _resumenService.ExportarDatosExcelAsync(request);

            return Ok();
        }
    }
}
