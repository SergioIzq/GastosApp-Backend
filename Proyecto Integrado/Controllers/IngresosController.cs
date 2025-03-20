using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.IngresoServicio;
using Microsoft.AspNetCore.Authorization;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/ingreso")]
    [Authorize]
    public class IngresoController : BaseController<Ingreso>
    {
        private readonly IIngresoServicio _ingresoService;

        public IngresoController(IIngresoServicio ingresoService) : base(ingresoService)
        {
            _ingresoService = ingresoService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _ingresoService.GetCantidadAsync<Ingreso>(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }


        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] Ingreso entity)
        {
            var createdEntity = await _ingresoService.CreateAsync(entity);

            var message = $"{typeof(Ingreso).Name} creado correctamente";

            var response = new ResponseOne<Ingreso>(createdEntity, message);

            return Ok(response);

        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, [FromBody] Ingreso entity)
        {

            await _ingresoService.UpdateAsync(id, entity);
            return Ok(new { message = $"{typeof(Ingreso).Name} actualizado correctamente" });

        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] Excel<IngresoDto> res)
        {

            _ingresoService.ExportarDatosExcelAsync(res);

            return Ok();
        }
    }
}
