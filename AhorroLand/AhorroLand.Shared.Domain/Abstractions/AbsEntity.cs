namespace AhorroLand.Shared.Domain.Abstractions;

public abstract class AbsEntity
{
    protected AbsEntity(Guid id)
    {
        Id = id;
        FechaCreacion = DateTime.Now;
    }

    public virtual Guid Id { get; init; }
    public virtual DateTime FechaCreacion { get; init; }
}
