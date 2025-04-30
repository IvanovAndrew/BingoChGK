using MediatR;

namespace Bingo.Application.GetBingoDescription;

public class BingoNotFoundEvent : INotification
{
    public long ChatId { get; init; }
    public string? BingoText { get; init; }
    public int? ReplyTo { get; init; }
}

public class BingoNotFoundEventHandler(ITelegramBot telegramBot) : INotificationHandler<BingoNotFoundEvent>
{
    public async Task Handle(BingoNotFoundEvent notification, CancellationToken cancellationToken)
    {
        await telegramBot.SendTextMessageAsync(notification.ChatId, $"Would you like to add bingo?",
            buttons: new TelegramButton[]
            {
                new() { Text = "Yes", Callback = $"/add {notification.BingoText}" },
                new() { Text = "No", Callback = "/skip" },
            }, replyToMessageId:notification.ReplyTo, cancellationToken: cancellationToken);
    }
}