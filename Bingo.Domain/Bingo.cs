namespace Bingo.Domain;

public class Bingo : AggregateRoot
{
    public string Text { get; internal set;}

    internal Bingo(int id, string text, string? description)
    {
        Id = id;
        Text = text;
        Description = description;
    }
    
    public string? Description { get; private set; }

    public string? InstantViewUrl { get; internal set; }

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
        
        AddEvent(new DescriptionUpdatedDomainEvent() {BingoId = Id, Bingo = Text, UpdatedBy = updatedBy});
    }

    public override string ToString()
    {
        return $"ID = {Id} Text = {Text}";
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