namespace Bingo.Domain;

public class User
{
    public long Id { get; init; }
    public bool CanRemoveQuestion { get; init; }
    public bool CanAddBingo { get; init; }
}