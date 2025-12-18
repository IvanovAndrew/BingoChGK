namespace Bingo.Domain;

public class User : AggregateRoot
{
    public long Id { get; init; }
    public bool IsActive { get; private set; }
    public bool CanRemoveQuestion { get; init; }
    public bool CanAddBingo { get; init; }

    public User(long id, bool isActive, bool isAdmin)
    {
        Id = id;
        IsActive = isActive;
        CanRemoveQuestion = isAdmin;
        CanAddBingo = isAdmin;
    }

    public void MakeActive()
    {
        if (IsActive)
            return;
        
        IsActive = true;
        AddEvent(new UserActivityChangedDomainEvent(){Id = Id, IsActive = true});
    }

    public void MakeInactive()
    {
        if (!IsActive)
            return;
        
        IsActive = false;
        AddEvent(new UserActivityChangedDomainEvent(){Id = Id, IsActive = false});
    }
}

public record UserActivityChangedDomainEvent : IDomainEvent
{
    public long Id { get; init; }
    public bool IsActive { get; init; }
}