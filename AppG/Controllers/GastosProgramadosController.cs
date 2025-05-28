using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using Microsoft.AspNetCore.Authorization;
using AppG.BBDD.Respuestas;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/gastoProgramado")]
    [Authorize]
    public class GastosProgramadosController : BaseController<GastoProgramado>
    {
        private readonly IGastoProgramadoServicio _gastoProgramadoService;

        public GastosProgramadosController(IGastoProgramadoServicio gastoProgramadoService) : base(gastoProgramadoService)
        {
            _gastoProgramadoService = gastoProgramadoService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _gastoProgramadoService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }


        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] GastoProgramado entity)
        {

            var createdEntity = await _gastoProgramadoService.CreateAsync(entity);
            var message = $"Gasto programado creado correctamente";

            var response = new ResponseOne<GastoProgramado>(createdEntity, message);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, [FromBody] GastoProgramado entity)
        {

            await _gastoProgramadoService.UpdateAsync(id, entity);
            return Ok(new { message = $"Gasto programado con ID {id} actualizado correctamente" });
        }

        [HttpGet("getNewGasto/{idUsuario}")]
        public async Task<IActionResult> GetNewGasto(int idUsuario)
        {
            var newGasto = await _gastoProgramadoService.GetNewGastoAsync(idUsuario);

            return Ok(newGasto);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetGastoById(int id)
        {
            var gastoById = await _gastoProgramadoService.GetGastoByIdAsync(id);

            return Ok(gastoById);
        }
    }
}
