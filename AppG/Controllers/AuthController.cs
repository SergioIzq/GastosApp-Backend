using AppG.Entidades.BBDD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NHibernate.Linq;
using AppG.Exceptions;
using System.Net.Mail;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISessionFactory _sessionFactory;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;

    public AuthController(ISessionFactory sessionFactory, IConfiguration configuration, EmailService emailService)
    {
        _sessionFactory = sessionFactory;
        _configuration = configuration;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Usuario usuario)
    {
        IList<string> errorMessages = new List<string>();

        using (var session = _sessionFactory.OpenSession())
        {
            var user = await session.Query<Usuario>()
                .SingleOrDefaultAsync(u => u.Correo == usuario.Correo);

            if (user == null)
            {
                errorMessages.Add($"El correo '{usuario.Correo}' no está registrado.");
            }
            else if (!VerifyPassword(usuario.Contrasena, user.Contrasena))
            {
                errorMessages.Add($"Contraseña incorrecta.");
            }

            if (!user!.Activo)
            {
                throw new UnauthorizedAccessException("Error en login.");
            }

            if (errorMessages.Count > 0)
            {
                throw new CustomUnauthorizedAccessException(errorMessages);
            }

            var token = GenerateJwtToken(user!);
            return Ok(new { token });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Usuario usuario)
    {
        IList<string> errorMessages = new List<string>();

        using (var session = _sessionFactory.OpenSession())
        {
            // Verificar si el usuario ya existe
            var existingUser = session.Query<Usuario>()
                .SingleOrDefault(u => u.Correo == usuario.Correo);

            if (existingUser != null)
            {
                errorMessages.Add($"El correo '{usuario.Correo}' ya está registrado.");

                throw new ValidationException(errorMessages);
            }

            // Hash de la contraseña
            var hasher = new PasswordHasher<Usuario>();
            var hashedPassword = hasher.HashPassword(usuario, usuario.Contrasena);
            usuario.Contrasena = hashedPassword;
            usuario.TokenConfirmacion = Guid.NewGuid().ToString();
            usuario.Activo = false;
            // Guardar el nuevo usuario
            using (var transaction = session.BeginTransaction())
            {
                session.Save(usuario);
                await transaction.CommitAsync();
            }
            try
            {
                var baseUrl = "https://ahorroland.sergioizq.es";
#if DEBUG
                baseUrl = "http://localhost:4200";
#endif
                var confirmUrl = $"{baseUrl}/auth/confirmar-correo?token={usuario.TokenConfirmacion}";

                await _emailService.SendEmailAsync(
                    usuario.Correo,
                    "Bienvenido a Ahorroland",
                    $@"
                    <html>
                      <body style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
                        <h1>Gracias por registrarte en Ahorroland</h1>
                        <p>Estamos felices de tenerte aquí. Por favor accede al siguiente enlace para verificar y activar su cuenta:</p>
                        <p><a href='{confirmUrl}' target='_blank' style='color: #1a73e8; text-decoration: none;'>Confirmar mi cuenta</a></p>
                      </body>
                    </html>
                    "
                    );
            }
            catch (SmtpException)
            {
                throw new SmtpException("Ha ocurrido un error al enviar correo");
            }

            return Ok(new { mensaje = "Usuario creado correctamente." });
        }
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(720);
        var expirationUnix = new DateTimeOffset(expirationTime).ToUnixTimeSeconds();

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Exp, expirationUnix.ToString()),

        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("F7o/dfNfO5AqZbHkLXM6z5Zm8DZpX0m6v7KD0tJr0uI="));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "GastosApp",
            audience: "GastosApp",
            claims: claims,
            expires: expirationTime,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var hasher = new PasswordHasher<Usuario>();
        var result = hasher.VerifyHashedPassword(null!, storedHash, password);
        return result == PasswordVerificationResult.Success;
    }

    [HttpGet("confirmar-correo")]
    public IActionResult ConfirmarCorreo([FromQuery] string token)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            var usuario = session.Query<Usuario>().SingleOrDefault(u => u.TokenConfirmacion == token);
            if (usuario == null)
            {
                return BadRequest("Token inválido o expirado.");
            }

            usuario.Activo = true;
            usuario.TokenConfirmacion = null; // Limpia el token
            session.Update(usuario);
            transaction.Commit();

            return Ok(new { mensaje = "Correo confirmado correctamente." });
        }
    }

}
