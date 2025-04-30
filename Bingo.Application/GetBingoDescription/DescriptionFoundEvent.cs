using MediatR;

namespace Bingo.Application.GetBingoDescription;

public record DescriptionFoundEvent(long ChatId, int BingoId, string Description) : INotification
{
}

public class DescriptionFoundEventHandler(ITelegramBot telegramBot) : INotificationHandler<DescriptionFoundEvent>
{
    public async Task Handle(DescriptionFoundEvent notification, CancellationToken cancellationToken)
    {
        await telegramBot.SendTextMessageAsync(notification.ChatId,
            notification.Description, buttons:
            [
                new() { Text = "Show a question", Callback = $"/question {notification.BingoId}" },
                // new() { Text = "Add synonyms", Callback = $"/addsynonyms {bingoId}" },
                new() { Text = "Next bingo", Callback = "/random" }
            ], cancellationToken: cancellationToken);
    }
}