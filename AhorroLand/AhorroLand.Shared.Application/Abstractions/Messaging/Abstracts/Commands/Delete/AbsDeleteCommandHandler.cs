using AhorroLand.Shared.Application.Abstractions.Servicies;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Abstractions.Results;
using AhorroLand.Shared.Domain.Interfaces;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using MediatR;

namespace AhorroLand.Shared.Application.Abstractions.Messaging.Abstracts.Commands
{
    /// <summary>
    /// Handler genérico para eliminar entidades.
    /// Busca la entidad, elimina del repositorio, persiste y limpia la caché.
    /// ✅ OPTIMIZADO: Usa el repositorio de escritura para obtener la entidad con tracking.
    /// </summary>
    public abstract class DeleteCommandHandler<TEntity, TCommand>
        : AbsCommandHandler<TEntity>, IRequestHandler<TCommand, Result>
        where TEntity : AbsEntity
        where TCommand : AbsDeleteCommand<TEntity>
    {
        // Se inyectan las mismas dependencias que el AbsCommandHandler
        public DeleteCommandHandler(
            IUnitOfWork unitOfWork,
            IWriteRepository<TEntity> writeRepository,
            ICacheService cacheService)
            : base(unitOfWork, writeRepository, cacheService)
        {
        }

        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            // 1. 🔧 FIX: Usar el repositorio de escritura para obtener la entidad con tracking
            var entity = await _writeRepository.GetByIdAsync(command.Id, cancellationToken);

            if (entity is null)
            {
                return Result.Failure(Error.NotFound($"Entidad {typeof(TEntity).Name} con ID '{command.Id}' no encontrada para eliminación."));
            }

            // 2. Persistencia: Usar el método base, que maneja la eliminación, SaveChanges y Cache Invalidation
            var result = await DeleteAsync(entity, cancellationToken);

            // 3. Devolver el resultado de la operación
            return result;
        }
    }
}