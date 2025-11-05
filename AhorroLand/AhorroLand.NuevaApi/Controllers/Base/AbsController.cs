using AhorroLand.Shared.Domain.Abstractions.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers.Base
{

    [ApiController]
    public abstract class AbsController : ControllerBase
    {
        protected readonly ISender _sender;

        protected AbsController(ISender sender)
        {
            _sender = sender;
        }

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return HandleFailure(result.Error);
        }

        // Sobrecarga para Resultados sin valor (como en Update o Delete)
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                // Un 'Update' o 'Delete' exitoso no devuelve contenido
                return NoContent();
            }

            return HandleFailure(result.Error);
        }

        // Método para manejar un 'Create' exitoso (devuelve 201 Created)
        protected IActionResult HandleResultForCreation<T>(Result<T> result, string actionName, object routeValues)
        {
            if (result.IsSuccess)
            {
                return CreatedAtAction(actionName, routeValues, result.Value);
            }

            return HandleFailure(result.Error);
        }

        private IActionResult HandleFailure(Error error)
        {
            return Ok();
        }
    }
}
