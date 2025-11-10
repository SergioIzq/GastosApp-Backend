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

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}