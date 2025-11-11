using AhorroLand.Infrastructure.Persistence.Command;
using AhorroLand.Shared.Domain.Abstractions;
using AhorroLand.Shared.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

public abstract class AbsWriteRepository<T> : IWriteRepository<T> where T : AbsEntity
{
    protected readonly AhorroLandDbContext _context;

    public AbsWriteRepository(AhorroLandDbContext context)
    {
        _context = context;

        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public virtual void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public virtual async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Actualiza una entidad verificando primero que existe en la base de datos.
    /// </summary>
    public virtual void Update(T entity)
    {
        // Verificar si la entidad existe en la base de datos
        var exists = _context.Set<T>().Any(e => e.Id == entity.Id);

        if (!exists)
        {
            throw new InvalidOperationException(
                $"No se puede actualizar la entidad {typeof(T).Name} con Id '{entity.Id}' porque no existe en la base de datos.");
        }

        _context.Set<T>().Update(entity);
    }

    /// <summary>
    /// Actualiza una entidad de forma asíncrona verificando primero que existe en la base de datos.
    /// </summary>
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Verificar si la entidad existe en la base de datos
        var exists = await _context.Set<T>().AnyAsync(e => e.Id == entity.Id, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                $"No se puede actualizar la entidad {typeof(T).Name} con Id '{entity.Id}' porque no existe en la base de datos.");
        }

        _context.Set<T>().Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}