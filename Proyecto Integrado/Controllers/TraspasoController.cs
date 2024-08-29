using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using AppG.Servicio;
using static AppG.Servicio.TraspasoServicio;
using Microsoft.AspNetCore.Authorization;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/traspaso")]
    [Authorize]
    public class TraspasoController : BaseController<Traspaso>
    {
        private readonly ITraspasoServicio _traspasoService;

        public TraspasoController(ITraspasoServicio traspasoService) : base(traspasoService)
        {
            _traspasoService = traspasoService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _traspasoService.GetCantidadAsync<Traspaso>(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpPost("realizarTraspaso")]
        public async Task<IActionResult> RealizarTraspaso(Traspaso entity)
        {

            var createdEntity = await _traspasoService.RealizarTraspaso(entity);

            var message = $"{typeof(Traspaso).Name} creado correctamente";

            var response = new ResponseOne<Traspaso>(createdEntity, message);

            return Ok(response);
        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] Excel<TraspasoDto> res)
        {

            _traspasoService.ExportarDatosExcelAsync(res);

            return Ok();
        }

    }
}
