using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

/// <summary>
/// Comando base genérico para operaciones de Eliminación.
/// </summary>
/// <typeparam name="TEntity">La Entidad de Dominio que se va a eliminar.</typeparam>
// Devuelve un Result<Unit> o simplemente Result para indicar éxito o fallo.
public abstract record AbsDeleteCommand<TEntity>(Guid Id) : IRequest<Result>
    where TEntity : AbsEntity;