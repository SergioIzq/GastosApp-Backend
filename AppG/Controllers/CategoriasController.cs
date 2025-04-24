using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Exceptions;
using AppG.Servicio;
using static AppG.Servicio.GastoServicio;
using static AppG.Servicio.CategoriaServicio;
using Microsoft.AspNetCore.Authorization;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/categoria")]
    [Authorize]
    public class CategoriaController : BaseController<Categoria>
    {
        private readonly ICategoriaServicio _categoriaService;

        public CategoriaController(ICategoriaServicio categoriaService) : base(categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _categoriaService.GetCantidadAsync<Categoria>(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                // Se devuelve el error directamente, pero el middleware se encargará de manejarlo
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpGet("getCategorias/{idUsuario}")]
        public async Task<IActionResult> GetCategorias(int idUsuario)
        {
            var result = await _categoriaService.GetAllAsync<Categoria>(idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                // Se devuelve el error directamente, pero el middleware se encargará de manejarlo
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        public override async Task<IActionResult> Create([FromBody] Categoria entity)
        {
            // Se lanza la excepción al middleware en lugar de manejarla aquí
            var createdEntity = await _categoriaService.CreateAsync(entity);

            var message = $"{typeof(Categoria).Name} creado correctamente";

            var response = new ResponseOne<Categoria>(createdEntity, message);

            return Ok(response);
        }

        public override async Task<IActionResult> Update(int id, [FromBody] Categoria entity)
        {
            // Se lanza la excepción al middleware en lugar de manejarla aquí
            await _categoriaService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(Categoria).Name} actualizado correctamente" });
        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] Excel<CategoriaDto> res)
        {

            _categoriaService.ExportarDatosExcelAsync(res);

            return Ok();
        }
    }
}
