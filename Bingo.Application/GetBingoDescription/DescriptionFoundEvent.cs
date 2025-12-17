using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.GetBingoDescription;

public record DescriptionFoundEvent(long ChatId, int BingoId, string Description) : INotification
{
}

public class DescriptionFoundEventHandler(ITelegramBot telegramBot, ILogger<DescriptionFoundEventHandler> logger) : INotificationHandler<DescriptionFoundEvent>
{
    public async Task Handle(DescriptionFoundEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await telegramBot.SendTextMessageAsync(notification.ChatId,
                notification.Description, buttons:
                [
                    TelegramButton.ShowAQuestionButton(notification.BingoId),
                    // new() { Text = "Add synonyms", Callback = $"/addsynonyms {bingoId}" },
                    TelegramButton.ShowRandomBingoButton("Next bingo")
                ], cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError($"{nameof(GetBingoDescriptionCommandHandler )} {e.ToString()}");
        }
    }
}