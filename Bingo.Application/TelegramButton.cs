namespace Bingo.Application;

public class TelegramButton
{
    public string Text { get; private init; }
    public string Callback { get; private init; }
    
    public static TelegramButton SkipButton(string text = "OK") => new(){Text = text, Callback = "/skip"}; 
    public static TelegramButton ShowAQuestionButton(int bingoId, string text = "Show a question") => new(){Text = text, Callback = $"/question {bingoId}"}; 
    public static TelegramButton EditDescriptionButton(int bingoId, string text = "Edit the description") => new(){Text = text, Callback = $"/editdescription {bingoId}"};

    public static TelegramButton AddBingoButton(string bingoText, string text = "Add a bingo") => new()
        { Text = text, Callback = $"/add {bingoText}" };

    public static TelegramButton ShowRandomBingoButton(string text) => new() { Text = text, Callback = "/random" };

    public static TelegramButton DeleteQuestionButton(int bingoId, int questionId, string text) => new() { Text = text, Callback = $"/deletequestion {questionId} bingo {bingoId}" };
}