using Microsoft.AspNetCore.Mvc;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using static AppG.Servicio.ClienteServicio;


namespace AppG.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : BaseController<Cliente>
    {
        private readonly IClienteServicio _clienteService;

        public ClienteController(IClienteServicio clienteService) : base(clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("getClientes/{idUsuario}")]
        public async Task<IActionResult> GetClientes(int idUsuario)
        {
            var result = await _clienteService.GetAllAsync<Cliente>(idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        [HttpGet("getCantidad")]
        public async Task<IActionResult> GetCantidad(int page, int size, int idUsuario)
        {
            var result = await _clienteService.GetCantidadAsync<Cliente>(page, size, idUsuario);

            if (result is IDictionary<string, object> errorResult && errorResult.ContainsKey("Error"))
            {
                return StatusCode(500, errorResult);
            }

            return Ok(result);
        }

        public override async Task<IActionResult> Create([FromBody] Cliente entity)
        {
            var createdEntity = await _clienteService.CreateAsync(entity);

            var message = $"{typeof(Cliente).Name} creado correctamente";

            var response = new ResponseOne<Cliente>(createdEntity, message);

            return Ok(response);

        }

        public override async Task<IActionResult> Update(int id, [FromBody] Cliente entity)
        {

            await _clienteService.UpdateAsync(id, entity);

            return Ok(new { message = $"{typeof(Cliente).Name} actualizado correctamente" });
        }

        [HttpPost("exportExcel")]
        public IActionResult ExportExcel([FromBody] Excel<ClienteDto> res)
        {

            _clienteService.ExportarDatosExcelAsync(res);

            return Ok();
        }

    }
}
