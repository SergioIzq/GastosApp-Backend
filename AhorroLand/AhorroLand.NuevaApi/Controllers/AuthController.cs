using AhorroLand.Application.Features.Auth.Commands.ConfirmEmail;
using AhorroLand.Application.Features.Auth.Commands.Login;
using AhorroLand.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorroLand.NuevaApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="command">Datos del usuario a registrar.</param>
    /// <returns>Mensaje de confirmación.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { mensaje = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Inicia sesión y devuelve un token JWT.
    /// </summary>
    /// <param name="command">Credenciales de inicio de sesión.</param>
    /// <returns>Token JWT y fecha de expiración.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return Unauthorized(new { mensaje = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Confirma el correo electrónico del usuario.
    /// </summary>
    /// <param name="token">Token de confirmación.</param>
    /// <returns>Mensaje de confirmación.</returns>
    [HttpGet("confirmar-correo")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmarCorreo([FromQuery] string token)
    {
        var command = new ConfirmEmailCommand(token);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(new { mensaje = result.Error.Message });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Endpoint de prueba que requiere autenticación.
    /// </summary>
    [HttpGet("test-auth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return Ok(new
        {
            mensaje = "Autenticación exitosa",
            userId,
            email
        });
    }
}
