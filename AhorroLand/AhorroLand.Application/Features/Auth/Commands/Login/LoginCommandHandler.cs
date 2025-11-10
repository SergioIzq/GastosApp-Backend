using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging;
using AhorroLand.Shared.Application.Interfaces;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.ValueObjects;

namespace AhorroLand.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IUsuarioReadRepository _usuarioReadRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUsuarioReadRepository usuarioReadRepository,
   IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioReadRepository = usuarioReadRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar usuario por correo
            var emailVO = new Email(request.Correo);
            var usuario = await _usuarioReadRepository.GetByEmailAsync(emailVO, cancellationToken);

            if (usuario == null)
            {
                return Result.Failure<LoginResponse>(new Error("Auth.InvalidCredentials", "Credenciales inválidas."));
            }

            // 2. Verificar que el usuario esté activo
            if (!usuario.Activo)
            {
                return Result.Failure<LoginResponse>(new Error("Auth.AccountNotActivated", "La cuenta no ha sido activada. Por favor, confirma tu correo electrónico."));
            }

            // 3. Verificar la contraseña
            var isPasswordValid = _passwordHasher.VerifyPassword(request.Contrasena, usuario.ContrasenaHash.Value);

            if (!isPasswordValid)
            {
                return Result.Failure<LoginResponse>(new Error("Auth.InvalidCredentials", "Credenciales inválidas."));
            }

            // 4. Generar el token JWT
            var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(usuario);

            return Result.Success(new LoginResponse(token, expiresAt));
        }
        catch (Exception ex)
        {
            return Result.Failure<LoginResponse>(new Error("Auth.LoginError", ex.Message));
        }
    }
}
