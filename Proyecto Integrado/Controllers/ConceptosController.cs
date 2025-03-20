using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.ConceptoServicio;
using Microsoft.AspNetCore.Authorization;


namespace AppG.Controllers
{
    [ApiController]
    [Route("api/concepto")]
    [Authorize]
    public class ConceptoController : BaseController<Concepto>
    {
        private readonly IConceptoServicio _conceptoService;

        public ConceptoController(IConceptoServicio conceptoService) : base(conceptoService)
        {
            _conceptoService = conceptoService;
        }

        [HttpGet("getConceptos/{idUsuario}")]
        public async Task<IActionResult> GetConceptos(int idUsuario)
        {
            var result = await _conceptoService.GetAllAsync<Concepto>(idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _conceptoService.GetCantidadAsync<Concepto>(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        public override async Task<IActionResult> Create([FromBody] Concepto entity)
        {

            var createdEntity = await _conceptoService.CreateAsync(entity);

            var message = $"{typeof(Concepto).Name} creado correctamente";

            var response = new ResponseOne<Concepto>(createdEntity, message);

            return Ok(response);

        }

        public override async Task<IActionResult> Update(int id, [FromBody] Concepto entity)
        {

            await _conceptoService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(Concepto).Name} actualizado correctamente" });

        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] Excel<ConceptoDto> res)
        {

            _conceptoService.ExportarDatosExcelAsync(res);

            return Ok();
        }

    }
}
