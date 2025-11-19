using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;

/// <summary>
/// Handler base para comandos de actualización.
/// ✅ OPTIMIZADO: Usa el repositorio de escritura para obtener la entidad con tracking.
/// </summary>
public abstract class AbsUpdateCommandHandler<TEntity, TDto, TCommand>
    : AbsCommandHandler<TEntity>, IRequestHandler<TCommand, Result<TDto>>
    where TEntity : AbsEntity
    where TCommand : AbsUpdateCommand<TEntity, TDto>
{
    public AbsUpdateCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<TEntity> writeRepository,
        ICacheService cacheService)
        : base(unitOfWork, writeRepository, cacheService)
    {
    }

    // 🔑 MÉTODO ABSTRACTO: Obliga al Command Handler concreto a implementar la lógica de actualización
    protected abstract void ApplyChanges(TEntity entity, TCommand command);

    public async Task<Result<TDto>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        // 1. 🔧 FIX: Usar el repositorio de escritura para obtener la entidad con tracking
        var entity = await _writeRepository.GetByIdAsync(command.Id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure<TDto>(Error.NotFound($"Entidad {typeof(TEntity).Name} con ID '{command.Id}' no encontrada."));
        }

        // 2. Aplicar la lógica de actualización de dominio llamando al método abstracto
        try
        {
            ApplyChanges(entity, command);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<TDto>(
                Error.Validation($"Error de dominio al actualizar {typeof(TEntity).Name}: {ex.Message}")
            );
        }

        // 3. Persistencia: Usar el método base, que maneja SaveChanges y Cache Invalidation
        var result = await UpdateAsync(entity, cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<TDto>(result.Error);
        }

        // 4. Mapeo: Mapear la entidad actualizada a DTO para la respuesta
        var dto = entity.Adapt<TDto>();

        return Result.Success(dto);
    }
}
