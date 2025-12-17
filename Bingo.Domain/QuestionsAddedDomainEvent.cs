namespace Bingo.Domain;

public record QuestionsAddedDomainEvent : IDomainEvent
{
    
}

public record QuestionRemovedDomainEvent : IDomainEvent
{
    public int BingoId { get; init; }
    public int QuestionId { get; init; }
    public long DeletedBy { get; init; }
}

public record DescriptionUpdatedDomainEvent : IDomainEvent
{
    public int BingoId { get; init; }
    public string Bingo { get; init; }
    public long UpdatedBy { get; init; }
}

public record BingoDescriptionShownDomainEvent : IDomainEvent
{
    
}

public record QuestionAskedDomainEvent : IDomainEvent
{
    
}