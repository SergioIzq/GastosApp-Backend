using AhorroLand.Shared.Domain.Interfaces;

namespace AhorroLand.Infrastructure.Persistence.Command;

public class UnitOfWork : IUnitOfWork
{
    private readonly AhorroLandDbContext _context;

    public UnitOfWork(AhorroLandDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
