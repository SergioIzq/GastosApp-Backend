using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using AppG.Servicio;
using Microsoft.AspNetCore.Authorization;

namespace AppG.Controllers
{
    public abstract class BaseController<T> : ControllerBase where T : class
    {
        protected readonly IBaseServicio<T> _gastoService;
        private readonly JsonSerializerOptions _jsonOptions;

        public BaseController(IBaseServicio<T> gastoService)
        {
            _gastoService = gastoService ?? throw new ArgumentNullException(nameof(gastoService));
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true
            };
        }

        protected IActionResult OkJson(object value)
        {
            // Crear JsonSerializerOptions sin PropertyNamingPolicy para conservar la capitalización original
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true
                // No se especifica PropertyNamingPolicy, así que se usa el comportamiento predeterminado
            };

            return new JsonResult(value, options);
        }

        protected IActionResult OkJson(object value, JsonSerializerOptions options)
        {
            // Crear JsonSerializerOptions sin PropertyNamingPolicy para conservar la capitalización original
            options.PropertyNamingPolicy = null; // Asegurarse de que no se aplique una política de nombres
            return new JsonResult(value, options);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            try
            {
                var entity = await _gastoService.GetByIdAsync(id);
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
                var createdEntity = await _gastoService.CreateAsync(entity);
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
                await _gastoService.UpdateAsync(id, entity);
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
                await _gastoService.DeleteAsync(id);
                return OkJson(new { message = $"{typeof(T).Name} con ID {id} eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar entidad: {ex.Message}");
            }
        }

    }
}
