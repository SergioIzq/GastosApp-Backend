using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.PersonaServicio;
using Microsoft.AspNetCore.Authorization;
using AppG.BBDD.Respuestas;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/persona")]
    [Authorize]
    public class PersonaController : BaseController<Persona>
    {
        private readonly IPersonaServicio _personaService;

        public PersonaController(IPersonaServicio personaService) : base(personaService)
        {
            _personaService = personaService;
        }

        [HttpGet("getPersonas/{idUsuario}")]
        public async Task<IActionResult> GetPersonas(int idUsuario)
        {
            var result = await _personaService.GetAllAsync(idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }


        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _personaService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        public override async Task<IActionResult> Create([FromBody] Persona entity)
        {

            var createdEntity = await _personaService.CreateAsync(entity);

            var message = $"{typeof(Persona).Name} creado correctamente";

            var response = new ResponseOne<Persona>(createdEntity, message);

            return Ok(response);
        }

        public override async Task<IActionResult> Update(int id, [FromBody] Persona entity)
        {

            await _personaService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(Persona).Name} actualizado correctamente" });

        }
    }
}
