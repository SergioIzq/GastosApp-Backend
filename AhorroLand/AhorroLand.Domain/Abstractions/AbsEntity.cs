namespace AhorroLandBackend.Domain.Abstractions;

public abstract class AbsEntity
{
    protected AbsEntity(Guid id)
    {
        Id = id;
        FechaCreacion = DateTime.Now;
    }

    private readonly List<IDomainEvent> _domainEvents = new();
    protected virtual Guid Id { get; init; }
    protected virtual DateTime FechaCreacion { get; set; }

    protected IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    protected void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
