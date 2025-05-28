using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using Microsoft.AspNetCore.Authorization;
using AppG.BBDD.Respuestas;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/traspasoProgramado")]
    [Authorize]
    public class TraspasosProgramadosController : BaseController<TraspasoProgramado>
    {
        private readonly ITraspasoProgramadoServicio _traspasoProgramadoService;

        public TraspasosProgramadosController(ITraspasoProgramadoServicio traspasoProgramadoService) : base(traspasoProgramadoService)
        {
            _traspasoProgramadoService = traspasoProgramadoService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _traspasoProgramadoService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpPost]
        public override async Task<IActionResult> Create([FromBody] TraspasoProgramado entity)
        {

            var createdEntity = await _traspasoProgramadoService.CreateAsync(entity);
            var message = $"Traspaso programado creado correctamente";

            var response = new ResponseOne<TraspasoProgramado>(createdEntity, message);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, [FromBody] TraspasoProgramado entity)
        {

            await _traspasoProgramadoService.UpdateAsync(id, entity);
            return Ok(new { message = $"Traspaso programado con ID {id} actualizado correctamente" });
        }

        [HttpGet("getNewTraspaso/{idUsuario}")]
        public async Task<IActionResult> GetNewTraspaso(int idUsuario)
        {
            var newTraspaso = await _traspasoProgramadoService.GetNewTraspasoAsync(idUsuario);

            return Ok(newTraspaso);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetGastoById(int id)
        {
            var gastoById = await _traspasoProgramadoService.GetTraspasoByIdAsync(id);

            return Ok(gastoById);
        }
    }
}
