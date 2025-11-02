using AhorroLand.Shared.Domain.Abstractions.Results;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result> where TCommand : ICommand
{

}

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>> where TCommand : ICommand<TResponse>
{

}
