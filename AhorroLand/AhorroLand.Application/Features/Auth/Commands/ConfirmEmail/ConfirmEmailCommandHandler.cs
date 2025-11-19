using AhorroLand.Domain;
using AhorroLand.Shared.Application.Abstractions.Messaging;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;

namespace AhorroLand.Application.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand, ConfirmEmailResponse>
{
    private readonly IUsuarioReadRepository _usuarioReadRepository;
    private readonly IUsuarioWriteRepository _usuarioWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmEmailCommandHandler(
        IUsuarioReadRepository usuarioReadRepository,
        IUsuarioWriteRepository usuarioWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioReadRepository = usuarioReadRepository;
        _usuarioWriteRepository = usuarioWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Buscar usuario por token
            var usuario = await _usuarioReadRepository.GetByConfirmationTokenAsync(request.Token, cancellationToken);

            if (usuario == null)
            {
                return Result.Failure<ConfirmEmailResponse>(new Error("Auth.InvalidToken", "Enlace inválido o expirado. Si ya confirmaste tu correo, inicia sesión."));
            }

            // 2. Confirmar el usuario (lógica de dominio)
            usuario.Confirmar(request.Token);

            // 3. Actualizar en el repositorio
            _usuarioWriteRepository.Update(usuario);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new ConfirmEmailResponse("Correo confirmado correctamente. Ya puedes iniciar sesión."));
        }
        catch (Exception ex)
        {
            return Result.Failure<ConfirmEmailResponse>(new Error("Auth.ConfirmEmailError", ex.Message));
        }
    }
}

