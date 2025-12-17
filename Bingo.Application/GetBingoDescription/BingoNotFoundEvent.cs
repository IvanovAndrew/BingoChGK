using MediatR;

namespace Bingo.Application.GetBingoDescription;

public class BingoNotFoundEvent : INotification
{
    public long ChatId { get; init; }
    public string BingoText { get; init; }
    public int? ReplyTo { get; init; }
}

public class BingoNotFoundEventHandler(ITelegramBot telegramBot) : INotificationHandler<BingoNotFoundEvent>
{
    public async Task Handle(BingoNotFoundEvent notification, CancellationToken cancellationToken)
    {
        if (notification.BingoText.Length > 100)
        {
            await telegramBot.SendTextMessageAsync(notification.ChatId, "Something went wrong. Enter your bingo again",
                replyToMessageId:notification.ReplyTo, cancellationToken: cancellationToken);
            
            return;
        }
        
        await telegramBot.SendTextMessageAsync(notification.ChatId, $"Would you like to add bingo?",
            buttons:
            [
                TelegramButton.AddBingoButton(notification.BingoText, "Yes"),
                TelegramButton.SkipButton("No")
            ], replyToMessageId:notification.ReplyTo, cancellationToken: cancellationToken);
    }
}