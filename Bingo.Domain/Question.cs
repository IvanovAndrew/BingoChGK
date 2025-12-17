namespace Bingo.Domain;

public class Question : ValueObject<Question>
{
    public int GotQuestionId { get; init; }
    public string PackTitle { get; set; }
    public DateOnly Date { get; set; }
    public int Number { get; set; }
    public string Text { get; set; }
    public string? AdditionalMaterialText { get; set; }
    public string? AdditionalMaterialPictureUrl { get; set; }
    public string Answer { get; set; }
    public string AcceptedAnswer { get; set; }
    public string NotAcceptedAnswer { get; set; }
    public string Comment { get; set; }
    public string CommentPictureUrl { get; set; }
    public string Note { get; set; }
    public string Author { get; set; }
    public string Editor { get; set; }
    public string[] Sources { get; set; }
    public string[] Authors { get; set; }
    public List<int> Teams { get; set; }
    public List<int> CorrectAnswers { get; set; }

    protected override bool EqualsCore(Question other)
    {
        return GotQuestionId == other.GotQuestionId;
    }

    protected override int GetHashCodeCore()
    {
        return GotQuestionId.GetHashCode();
    }
}