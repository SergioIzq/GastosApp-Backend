using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging;
using AhorroLand.Shared.Application.Interfaces;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUsuarioWriteRepository _usuarioWriteRepository;
    private readonly IUsuarioReadRepository _usuarioReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailSender _emailSender;

    public RegisterCommandHandler(
        IUsuarioWriteRepository usuarioWriteRepository,
        IUsuarioReadRepository usuarioReadRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IEmailSender emailSender)
    {
        _usuarioWriteRepository = usuarioWriteRepository;
        _usuarioReadRepository = usuarioReadRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _emailSender = emailSender;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el correo no exista
            var emailVO = new Email(request.Correo);
            var existingUser = await _usuarioReadRepository.GetByEmailAsync(emailVO, cancellationToken);

            if (existingUser != null)
            {
                return Result.Failure<RegisterResponse>(new Error("Auth.EmailExists", $"El correo '{request.Correo}' ya está registrado."));
            }

            // 2. Hash de la contraseña
            var hashedPassword = _passwordHasher.HashPassword(request.Contrasena);
            var passwordHashVO = new PasswordHash(hashedPassword);

            // 3. Crear el usuario usando el método Factory del dominio
            var newUsuario = Usuario.Create(emailVO, passwordHashVO);

            // 4. Guardar en el repositorio
            await _usuarioWriteRepository.CreateAsync(newUsuario, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Enviar email de confirmación
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:4200";
            var confirmUrl = $"{baseUrl}/auth/confirmar-correo?token={newUsuario.TokenConfirmacion!.Value.Value}";

            await _emailSender.SendEmailAsync(
                        newUsuario.Correo.Value,
              "Bienvenido a AhorroLand",
                 $@"
      <html>
       <body style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
     <h1>Gracias por registrarte en AhorroLand</h1>
      <p>Estamos felices de tenerte aquí. Por favor accede al siguiente enlace para verificar y activar su cuenta:</p>
         <p><a href='{confirmUrl}' target='_blank' style='color: #1a73e8; text-decoration: none;'>Confirmar mi cuenta</a></p>
  </body>
      </html>
        ",
                   cancellationToken
                   );

            return Result.Success(new RegisterResponse("Usuario registrado correctamente. Por favor, confirma tu correo."));
        }
        catch (Exception ex)
        {
            return Result.Failure<RegisterResponse>(new Error("Auth.RegisterError", ex.Message));
        }
    }
}
