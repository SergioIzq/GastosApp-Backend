using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

/// <summary>
/// Handler genérico para crear entidades. 
/// Realiza persistencia, invalidación de caché y mapeo de respuesta.
/// </summary>
/// <typeparam name="TEntity">La Entidad de Dominio.</typeparam>
/// <typeparam name="TDto">El DTO de respuesta.</typeparam>
/// <typeparam name="TCommand">El tipo de comando concreto que hereda de AbsCreateTCommand.</typeparam>
public abstract class AbsCreateCommandHandler<TEntity, TDto, TCommand>
// Heredamos funcionalidad de persistencia y caché
    : AbsCommandHandler<TEntity>, IRequestHandler<TCommand, Result<TDto>>
    where TEntity : AbsEntity
    where TCommand : AbsCreateCommand<TEntity, TDto>
{
    public AbsCreateCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<TEntity> writeRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, cacheService)
    {
    }

    /// <summary>
    /// Método abstracto que debe ser implementado por el Handler concreto.
    /// Aquí se contiene la lógica de negocio para crear la entidad a partir del DTO.
    /// </summary>
    protected abstract TEntity CreateEntity(TCommand command);

    public async Task<Result<TDto>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        // 1. Lógica de Negocio: Crear la entidad (provista por la clase concreta)
        TEntity entity;
        try
        {
            entity = CreateEntity(command);
        }
        catch (Exception ex)
        {
            return Result.Failure<TDto>(
                Error.Validation($"Error al crear {typeof(TEntity).Name}: {ex.Message}")
            );
        }

        // 2. Persistencia: Usar el método base, que maneja SaveChanges y Cache Invalidation
        var result = await CreateAsync(entity, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<TDto>(result.Error);
        }

        // 3. Mapeo: Mapear la entidad persistida a DTO (Usando Mapster)
        var dto = result.Value.Adapt<TDto>();

        return Result.Success(dto);
    }
}