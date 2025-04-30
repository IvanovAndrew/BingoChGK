using MediatR;

namespace Bingo.Application.AddBingo;

public class NewBingoQuestionsDownloadedEvent : INotification
{
    public long ChatId { get; init; }
    public int MessageId { get; init; }
    public int QuestionsCount { get; init; }
    public int BingoId { get; init; }
    public string BingoWord { get; init; }
}

public class NewBingoQuestionsDownloadedEventHandler(ITelegramBot telegramBot) : INotificationHandler<NewBingoQuestionsDownloadedEvent>
{
    public async Task Handle(NewBingoQuestionsDownloadedEvent notification, CancellationToken cancellationToken)
    {
        await telegramBot.EditTextMessageAsync(
            notification.ChatId, 
            notification.MessageId,
            $"{notification.QuestionsCount} {(notification.QuestionsCount > 1 ? "questions" : "question")} for {notification.BingoWord} {(notification.QuestionsCount % 10 == 1 ? "has" : "have")} been added",
            buttons: new TelegramButton[]
            {
                new() { Text = "Edit the description", Callback = $"/editdescription {notification.BingoId}" },
                new() { Text = "Show a question", Callback = $"/question {notification.BingoId}" },
                new() { Text = "ОК", Callback = $"/skip" }
            }, cancellationToken: cancellationToken);
    }
}