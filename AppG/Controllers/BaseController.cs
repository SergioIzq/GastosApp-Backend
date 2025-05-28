using AppG.BBDD.Excel;
using AppG.BBDD.Respuestas;
using AppG.Entidades.BBDD;
using AppG.Servicio;
using AppG.Servicio.Base;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppG.Controllers
{
    public abstract class BaseController<T> : ControllerBase where T : Entidad, IExportable
    {
        protected readonly IBaseServicio<T> _baseService;
        private readonly JsonSerializerOptions _jsonOptions;

        public BaseController(IBaseServicio<T> baseService)
        {
            _baseService = baseService ?? throw new ArgumentNullException(nameof(baseService));
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true
            };
        }

        protected IActionResult OkJson(object value)
        {
            // Crear JsonSerializerOptions sin PropertyNamingPolicy para conservar la capitalizaci�n original
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true
                // No se especifica PropertyNamingPolicy, as� que se usa el comportamiento predeterminado
            };

            return new JsonResult(value, options);
        }

        protected IActionResult OkJson(object value, JsonSerializerOptions options)
        {
            // Crear JsonSerializerOptions sin PropertyNamingPolicy para conservar la capitalizaci�n original
            options.PropertyNamingPolicy = null; // Asegurarse de que no se aplique una pol�tica de nombres
            return new JsonResult(value, options);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            try
            {
                var entity = await _baseService.GetByIdAsync(id);
                if (entity == null)
                {
                    return NotFound($"Entidad con ID {id} no encontrada");
                }
                return OkJson(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar entidad: {ex.Message}");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] T entity)
        {
            try
            {
                var createdEntity = await _baseService.CreateAsync(entity);
                var message = $"{typeof(T).Name} creado correctamente";

                var response = new ResponseOne<T>(entity, message);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear entidad: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, [FromBody] T entity)
        {
            try
            {
                await _baseService.UpdateAsync(id, entity);
                return OkJson(new { message = $"{typeof(T).Name} con ID {id} actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar entidad: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _baseService.DeleteAsync(id);
                return OkJson(new { message = $"{typeof(T).Name} con ID {id} eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar entidad: {ex.Message}");
            }
        }

        [HttpPost("exportar")]
        public async Task<IActionResult> ExportarGenerico([FromBody] ExportarOpciones opciones, [FromServices] IExcelServicio excelServicio)
        {
            if (!typeof(IExportable).IsAssignableFrom(typeof(T)))
            {
                return BadRequest($"La entidad {typeof(T).Name} no soporta exportaci�n.");
            }

            var bytes = await excelServicio.ExportarExcelAsync<T>(opciones);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{opciones.NombreArchivo}.xlsx");
        }

    }
}
