namespace Bingo.Domain;

public class Question : ValueObject<Question>
{
    public int Id { get; init; }
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
    public string Note { get; set; }
    public string Author { get; set; }
    public string Editor { get; set; }
    public int BingoId { get; set; }
    public string[] Sources { get; set; }
    public string[] Authors { get; set; }
    protected override bool EqualsCore(Question other)
    {
        return BingoId == other.BingoId && Id == other.Id;
    }

    protected override int GetHashCodeCore()
    {
        return HashCode.Combine(BingoId, Id);
    }
}