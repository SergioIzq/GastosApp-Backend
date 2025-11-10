using AhorroLand.Shared.Application.Abstractions.Messaging;

namespace AhorroLand.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Correo,
    string Contrasena
) : ICommand<LoginResponse>;

public sealed record LoginResponse(
    string Token,
    DateTime ExpiresAt
);
