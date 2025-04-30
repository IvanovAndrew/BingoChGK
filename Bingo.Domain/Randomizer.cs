namespace BingoChGK;

public static class Randomizer
{
    private static readonly Random Random = new();
    
    public static T GetRandomElement<T>(IReadOnlyList<T> items)
    {
        var index = Random.Next(0, items.Count);
        return items[index];
    }
}