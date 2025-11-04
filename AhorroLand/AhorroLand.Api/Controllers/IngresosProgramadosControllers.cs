using AppG.BBDD.Respuestas;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/ingresoProgramado")]
    [Authorize]
    public class IngresosProgramadosController : BaseController<IngresoProgramado>
    {
        private readonly IIngresoProgramadoServicio _ingresoProgramadoService;

        public IngresosProgramadosController(IIngresoProgramadoServicio ingresoProgramadoService) : base(ingresoProgramadoService)
        {
            _ingresoProgramadoService = ingresoProgramadoService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _ingresoProgramadoService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] IngresoProgramado entity)
        {

            var createdEntity = await _ingresoProgramadoService.CreateAsync(entity);
            var message = $"Ingreso programado creado correctamente";

            var response = new ResponseOne<IngresoProgramado>(createdEntity, message);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, [FromBody] IngresoProgramado entity)
        {

            await _ingresoProgramadoService.UpdateAsync(id, entity);
            return Ok(new { message = $"Ingreso programado con ID {id} actualizado correctamente" });
        }

        [HttpGet("getNewIngreso/{idUsuario}")]
        public async Task<IActionResult> GetNewIngreso(int idUsuario)
        {
            var newIngreso = await _ingresoProgramadoService.GetNewIngresoAsync(idUsuario);

            return Ok(newIngreso);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetGastoById(int id)
        {
            var gastoById = await _ingresoProgramadoService.GetIngresoByIdAsync(id);

            return Ok(gastoById);
        }
    }
}
