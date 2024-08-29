using AppG.Entidades.BBDD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppG.Exceptions;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISessionFactory _sessionFactory;
    private readonly IConfiguration _configuration;

    public AuthController(ISessionFactory sessionFactory, IConfiguration configuration)
    {
        _sessionFactory = sessionFactory;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Usuario usuario)
    {
        IList<string> errorMessages = new List<string>();

        using (var session = _sessionFactory.OpenSession())
        {
            var user = session.Query<Usuario>()
                .SingleOrDefault(u => u.Correo == usuario.Correo);

            if (user == null)
            {
                errorMessages.Add($"El correo '{usuario.Correo}' no está registrado.");
            }
            else if (!VerifyPassword(usuario.Contrasena, user.Contrasena))
            {
                errorMessages.Add($"Contraseña '{usuario.Contrasena}' incorrecta.");
            }

            if (errorMessages.Count > 0)
            {
                throw new CustomUnauthorizedAccessException(errorMessages);
            }

            var token = GenerateJwtToken(user);
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

            // Guardar el nuevo usuario
            using (var transaction = session.BeginTransaction())
            {
                session.Save(usuario);
                await transaction.CommitAsync();
            }

            // Generar el token (opcional, puedes decidir si quieres emitir un token al registrar un usuario)
            var token = GenerateJwtToken(usuario);
            return Ok(new { token });
        }
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(120);
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
        var result = hasher.VerifyHashedPassword(null, storedHash, password);
        return result == PasswordVerificationResult.Success;
    }
}
