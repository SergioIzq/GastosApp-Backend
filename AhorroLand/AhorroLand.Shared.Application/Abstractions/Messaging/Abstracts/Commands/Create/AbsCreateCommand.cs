using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

/// <summary>
/// Comando base genérico para operaciones de Creación.
/// </summary>
/// <typeparam name="TEntity">La Entidad de Dominio que se va a crear.</typeparam>
/// <typeparam name="TDto">El DTO de respuesta que se espera.</typeparam>
// Hereda de IRequest<Result<TDto>> para el flujo de MediatR
public abstract record AbsCreateCommand<TEntity, TDto> : IRequest<Result<TDto>>
    where TEntity : AbsEntity
{

}