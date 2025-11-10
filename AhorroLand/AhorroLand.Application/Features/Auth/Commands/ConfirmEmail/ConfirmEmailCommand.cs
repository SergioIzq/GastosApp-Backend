using AhorroLand.Shared.Application.Abstractions.Messaging;

namespace AhorroLand.Application.Features.Auth.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Token
) : ICommand<ConfirmEmailResponse>;

public sealed record ConfirmEmailResponse(
    string Mensaje
);
