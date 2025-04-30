namespace Bingo.Domain;

public class QuestionsAddedDomainEvent : IDomainEvent
{
    
}

public class QuestionRemovedDomainEvent : IDomainEvent
{
    public int BingoId { get; init; }
    public int QuestionId { get; init; }
    public long DeletedBy { get; init; }
}

public class DescriptionUpdatedDomainEvent : IDomainEvent
{
    public string Bingo { get; init; }
    public long UpdatedBy { get; init; }
}

public class BingoDescriptionShownDomainEvent : IDomainEvent
{
    
}

public class QuestionAskedDomainEvent : IDomainEvent
{
    
}