namespace Bingo.Domain;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _events = [];

    protected void AddEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public IReadOnlyList<IDomainEvent> GetEvents()
    {
        return _events;
    }

    public void ClearEvents()
    {
        _events.Clear();
    }
}