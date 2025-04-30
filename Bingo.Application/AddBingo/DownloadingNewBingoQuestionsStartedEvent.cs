using MediatR;

namespace Bingo.Application.AddBingo;

public class DownloadingNewBingoQuestionsStartedEvent : INotification
{
    public long ChatId { get; init; }
    public int MessageId { get; init; }
}

public class DownloadingNewBingoQuestionsStartedEventHandler(ITelegramBot telegramBot) : INotificationHandler<DownloadingNewBingoQuestionsStartedEvent>
{
    public async Task Handle(DownloadingNewBingoQuestionsStartedEvent notification, CancellationToken cancellationToken)
    {
        await telegramBot.EditTextMessageAsync(notification.ChatId, notification.MessageId,
            "Downloading the questions...", cancellationToken: cancellationToken);
    }
}