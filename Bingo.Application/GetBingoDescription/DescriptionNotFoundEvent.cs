using MediatR;

namespace Bingo.Application.GetBingoDescription;

public class DescriptionNotFoundEvent : INotification
{
    public long ChatId { get; init; }
    public int BingoId { get; init; }
    public string? BingoText { get; init; }
}

public class DescriptionNotFoundEventHandler(ITelegramBot telegramBot) : INotificationHandler<DescriptionNotFoundEvent>
{
    public async Task Handle(DescriptionNotFoundEvent notification, CancellationToken cancellationToken)
    {
        await telegramBot.SendTextMessageAsync(notification.ChatId, $"There is no description for {notification.BingoText}",
            buttons:
            [
                TelegramButton.EditDescriptionButton(notification.BingoId, "Add a description"),
                TelegramButton.ShowAQuestionButton(notification.BingoId),
                TelegramButton.ShowRandomBingoButton("Next bingo")
            ], cancellationToken: cancellationToken);
    }
}