using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.TraspasoServicio;
using Microsoft.AspNetCore.Authorization;
using AppG.BBDD.Respuestas;

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
            var result = await _traspasoService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpPost("realizarTraspaso")]
        public async Task<IActionResult> RealizarTraspaso(Traspaso entity)
        {

            var createdEntity = await _traspasoService.RealizarTraspaso(entity, false);

            var message = $"{typeof(Traspaso).Name} creado correctamente";

            var response = new ResponseOne<Traspaso>(createdEntity, message);

            return Ok(response);
        }

        [HttpGet("getNewTraspaso/{idUsuario}")]
        public async Task<IActionResult> GetNewTraspaso(int idUsuario)
        {
            var newTraspaso = await _traspasoService.GetNewTraspasoAsync(idUsuario);

            return Ok(newTraspaso);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetTraspasoById(int id)
        {
            var traspasoById = await _traspasoService.GetTraspasoByIdAsync(id);

            return Ok(traspasoById);
        }

    }
}
