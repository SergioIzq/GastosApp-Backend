namespace AppG.Domain.Abstractions;

public abstract class AbsEntity
{
    protected AbsEntity()
    {
        Id = 0;
        FechaCreacion = DateTime.Now;
    }

    private readonly List<IDomainEvent> _domainEvents = new();
    protected virtual int Id { get; init; }
    protected virtual DateTime FechaCreacion { get; set; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
