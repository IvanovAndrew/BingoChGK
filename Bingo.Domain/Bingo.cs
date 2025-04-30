namespace Bingo.Domain;

public class Bingo : AggregateRoot
{
    private readonly List<Question> _questions = new();
    public int Id { get; internal set; }
    public string Text { get; internal set;}

    internal Bingo(int id, string text, string? description)
    {
        Id = id;
        Text = text;
        Description = description;
    }
    
    public string? Description { get; private set; }

    public string? InstantViewUrl { get; internal set; }

    public IReadOnlyList<Question> Questions => _questions;

    public DateOnly LastQuestionDateOrDefault =>
        _questions.Any() ? _questions.Max(q => q.Date) : DateOnly.MinValue;

    public void AddQuestions(List<Question> questions)
    {
        _questions.AddRange(questions);
        AddEvent(new QuestionsAddedDomainEvent());
    }

    public bool DeleteQuestion(int questionId, long deletedBy)
    {
        var questionToDelete = _questions.FirstOrDefault(q => q.Id == questionId);

        if (questionToDelete == null)
        {
            return false;
        }

        _questions.Remove(questionToDelete);
        AddEvent(new QuestionRemovedDomainEvent(){BingoId = Id, QuestionId = questionId, DeletedBy = deletedBy});
        return true;
    }

    public void UpdateDescription(string description, long updatedBy)
    {
        if (description.StartsWith("https://") || description.Contains("t.me"))
        {
            InstantViewUrl = description;
        }
        else
        {
            Description = description;
        }
        
        AddEvent(new DescriptionUpdatedDomainEvent() {Bingo = Text, UpdatedBy = updatedBy});
    }
}

public class BingoBuilder
{
    private readonly int _id;
    private readonly string _text;
    private readonly string? _description;
    private string? _instantUrl;

    public BingoBuilder(int id, string text, string? description)
    {
        _id = id;
        _text = text;
        _description = description;
    }

    public BingoBuilder WithInstantViewUrl(string instantUrl)
    {
        _instantUrl = instantUrl;
        return this;
    }

    public Bingo Build()
    {
        return new Bingo(_id, _text, _description)
        {
            InstantViewUrl = _instantUrl
        };
    }
}