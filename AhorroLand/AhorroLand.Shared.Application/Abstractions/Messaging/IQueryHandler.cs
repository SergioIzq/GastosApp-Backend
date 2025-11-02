using AhorroLand.Shared.Domain.Abstractions.Results;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging;
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>
{
}