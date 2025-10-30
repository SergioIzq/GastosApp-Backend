using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.GastoServicio;
using Microsoft.AspNetCore.Authorization;
using AppG.BBDD.Respuestas;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/gasto")]
    [Authorize]
    public class GastoController : BaseController<Gasto>
    {
        private readonly IGastoServicio _gastoService;

        public GastoController(IGastoServicio gastoService) : base(gastoService)
        {
            _gastoService = gastoService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _gastoService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        
        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] Gasto entity)
        {

            var createdEntity = await _gastoService.CreateAsync(entity, false);

            var message = $"{typeof(Gasto).Name} creado correctamente";

            var response = new ResponseOne<Gasto>(createdEntity, message);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, [FromBody] Gasto entity)
        {

            await _gastoService.UpdateAsync(id, entity);
            return Ok(new { message = $"{typeof(Gasto).Name} con ID {id} actualizado correctamente" });
        }

        [HttpGet("getNewGasto/{idUsuario}")]
        public async Task<IActionResult> GetNewGasto(int idUsuario)
        {
            var newGasto = await _gastoService.GetNewGastoAsync(idUsuario);            

            return Ok(newGasto);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetGastoById(int id)
        {
            var gastoById = await _gastoService.GetGastoByIdAsync(id);

            return Ok(gastoById);
        }
    }
}
