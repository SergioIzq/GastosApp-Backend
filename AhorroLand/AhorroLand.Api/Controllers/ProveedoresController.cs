using AppG.BBDD.Respuestas;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppG.Controllers
{
    [ApiController]
    [Route("api/proveedor")]
    [Authorize]
    public class ProveedorController : BaseController<Proveedor>
    {
        private readonly IProveedorServicio _proveedorService;

        public ProveedorController(IProveedorServicio proveedorService) : base(proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet("getProveedores/{idUsuario}")]
        public async Task<IActionResult> GetProveedores(int idUsuario)
        {
            var result = await _proveedorService.GetAllAsync(idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _proveedorService.GetCantidadAsync(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        public override async Task<IActionResult> Create([FromBody] Proveedor entity)
        {

            var createdEntity = await _proveedorService.CreateAsync(entity);

            var message = $"{typeof(Proveedor).Name} creado correctamente";

            var response = new ResponseOne<Proveedor>(createdEntity, message);

            return Ok(response);
        }

        public override async Task<IActionResult> Update(int id, [FromBody] Proveedor entity)
        {

            await _proveedorService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(Proveedor).Name} actualizado correctamente" });
        }

    }
}
