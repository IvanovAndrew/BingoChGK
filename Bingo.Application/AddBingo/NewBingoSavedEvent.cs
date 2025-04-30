using MediatR;

namespace Bingo.Application.AddBingo;

public class NewBingoSavedEvent : INotification
{
    public long ChatId { get; init; }
    public int MessageId { get; init; }
}

public class NewBingoSavedEventHandler(ITelegramBot telegramBot) : INotificationHandler<NewBingoSavedEvent>
{
    public async Task Handle(NewBingoSavedEvent notification, CancellationToken cancellationToken)
    {
        await telegramBot.EditTextMessageAsync(notification.ChatId, notification.MessageId, "Bingo has been added", cancellationToken: cancellationToken);
    }
}