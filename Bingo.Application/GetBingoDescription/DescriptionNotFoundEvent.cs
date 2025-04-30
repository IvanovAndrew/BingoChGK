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
            buttons: new TelegramButton[]
            {
                new() { Text = "Add a description", Callback = $"/editdescription {notification.BingoId}" },
                // new() { Text = "Add synonyms", Callback = $"/addsynonyms {bingoId}" },
                new() { Text = "Show a question", Callback = $"/question {notification.BingoId}" },
                new() { Text = "Next bingo", Callback = "/random" }
            }, cancellationToken: cancellationToken);
    }
}