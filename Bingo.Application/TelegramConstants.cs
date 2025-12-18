namespace Bingo.Application;

public static class TelegramConstants
{
    public static IReadOnlyDictionary<string, string> Commands = new Dictionary<string, string>()
    {
        ["/info"] = "Show the list of available commands",
        ["/subscribe"] = "Receive a random bingo every 6 hours (except at night)",
        ["/unsubscribe"] = "Unsubscribe from receiving random bingo",
        ["/random"] = "Get a random bingo fact",
    };
}