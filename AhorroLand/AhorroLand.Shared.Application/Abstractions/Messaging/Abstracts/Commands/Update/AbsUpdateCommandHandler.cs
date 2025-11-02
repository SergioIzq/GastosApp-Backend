using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Mapster;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands;
public abstract class AbsUpdateCommandHandler<TEntity, TDto, TCommand>
    : AbsCommandHandler<TEntity>, IRequestHandler<TCommand, Result<TDto>>
    where TEntity : AbsEntity
    where TCommand : AbsUpdateCommand<TEntity, TDto>
{
    private readonly IReadOnlyRepository<TEntity> _readRepository;

    public AbsUpdateCommandHandler(
        IUnitOfWork unitOfWork,
        IWriteRepository<TEntity> writeRepository,
        ICacheService cacheService,
        IReadOnlyRepository<TEntity> readRepository)
        : base(unitOfWork, writeRepository, cacheService)
    {
        _readRepository = readRepository;
    }

    // 🔑 MÉTODO ABSTRACTO: Obliga al Command Handler concreto a implementar la lógica de actualización
    protected abstract void ApplyChanges(TEntity entity, TCommand command);

    public async Task<Result<TDto>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        // 1. Buscar la entidad existente (debe ser trackeada por EF para actualizar)
        var entity = await _readRepository.GetByIdAsync(command.Id, asNoTracking: false, cancellationToken);

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
