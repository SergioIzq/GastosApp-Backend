using AhorroLand.Shared.Domain.Abstractions.Results;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{

}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}