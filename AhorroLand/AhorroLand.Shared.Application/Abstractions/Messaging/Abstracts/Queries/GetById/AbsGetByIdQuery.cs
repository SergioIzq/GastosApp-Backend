using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Queries
{
    /// <summary>
    /// Consulta base genérica para obtener una entidad por su ID.
    /// </summary>
    /// <typeparam name="TEntity">La Entidad de Dominio que se busca.</typeparam>
    /// <typeparam name="TDto">El DTO de respuesta que se espera (ya mapeado).</typeparam>
    public abstract record AbsGetByIdQuery<TEntity, TDto>(Guid Id) : IRequest<Result<TDto>>
        where TEntity : AbsEntity;
}