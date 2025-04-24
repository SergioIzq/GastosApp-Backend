using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.FormaPagoServicio;
using Microsoft.AspNetCore.Authorization;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/formapago")]
    [Authorize]
    public class FormaPagoController : BaseController<FormaPago>
    {
        private readonly IFormaPagoServicio _formaPagoService;

        public FormaPagoController(IFormaPagoServicio formaPagoService) : base(formaPagoService)
        {
            _formaPagoService = formaPagoService;
        }

        [HttpGet("getFormaPago/{idUsuario}")]
        public async Task<IActionResult> GetFormaPago(int idUsuario)
        {
            var result = await _formaPagoService.GetAllAsync<FormaPago>(idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _formaPagoService.GetCantidadAsync<FormaPago>(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        public override async Task<IActionResult> Create([FromBody] FormaPago entity)
        {

            var createdEntity = await _formaPagoService.CreateAsync(entity);

            var message = $"{typeof(FormaPago).Name} creado correctamente";

            var response = new ResponseOne<FormaPago>(createdEntity, message);

            return Ok(response);

        }

        public override async Task<IActionResult> Update(int id, [FromBody] FormaPago entity)
        {

            await _formaPagoService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(FormaPago).Name} actualizado correctamente" });

        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] Excel<FormaPagoDto> res)
        {

            _formaPagoService.ExportarDatosExcelAsync(res);

            return Ok();
        }
    }
}
