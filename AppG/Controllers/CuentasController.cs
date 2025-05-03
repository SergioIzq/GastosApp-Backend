using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.CuentaServicio;
using Microsoft.AspNetCore.Authorization;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/cuenta")]
    [Authorize]

    public class CuentaController : BaseController<Cuenta>
    {
        private readonly ICuentaServicio _cuentaService;

        public CuentaController(ICuentaServicio cuentaService) : base(cuentaService)
        {
            _cuentaService = cuentaService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _cuentaService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpGet("getCuentas/{idUsuario}")]
        public async Task<IActionResult> GetCuentas(int idUsuario)
        {
            var result = await _cuentaService.GetAllAsync(idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        public override async Task<IActionResult> Create([FromBody] Cuenta entity)
        {
            var createdEntity = await _cuentaService.CreateAsync(entity);

            var message = $"{typeof(Cuenta).Name} creado correctamente";

            var response = new ResponseOne<Cuenta>(createdEntity, message);

            return Ok(response);

        }

        public override async Task<IActionResult> Update(int id, [FromBody] Cuenta entity)
        {

            await _cuentaService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(Cuenta).Name} actualizado correctamente" });
        }

        public override async Task<IActionResult> Delete(int id)
        {

            await _cuentaService.DeleteAsync(id);

            return Ok(new { message = $"{typeof(Cuenta).Name} eliminada correctamente" });

        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] Excel<CuentaDto> res)
        {

            _cuentaService.ExportarDatosExcelAsync(res);

            return Ok();
        }

    }
}
