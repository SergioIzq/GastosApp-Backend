using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using Microsoft.AspNetCore.Authorization;


namespace AppG.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : BaseController<Usuario>
    {
        private readonly IUsuarioServicio _usuarioService;

        public UsuarioController(IUsuarioServicio usuarioService) : base(usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public override async Task<IActionResult> Update(int id, [FromBody] Usuario entity)
        {

            await _usuarioService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(Usuario).Name} actualizado correctamente" });
        }


        public override async Task<IActionResult> Delete(int id)
        {

            await _usuarioService.DeleteAsync(id);

            return Ok(new { message = $"{typeof(Usuario).Name} eliminado correctamente" });
        }

        [HttpGet("{id}")]
        public override Task<IActionResult> GetById(int id)
        {
            return base.GetById(id);  
        }
    }
}
