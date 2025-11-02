using AhorroLand.Shared.Domain.Abstractions;

namespace AhorroLand.Shared.Domain.Interfaces
{
    public interface IDomainValidator
    {
        Task<bool> ExistsAsync<TEntity>(Guid id) where TEntity : AbsEntity;
    }
}
