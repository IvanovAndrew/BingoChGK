using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.EditBingoDescription;

public class DescriptionUpdatedDomainEventHandler(ITelegramBot telegramBot, ILogger<DescriptionUpdatedDomainEventHandler> logger) : INotificationHandler<DescriptionUpdatedDomainEvent>
{
    public async Task Handle(DescriptionUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"{nameof(DescriptionUpdatedDomainEventHandler)} {notification}");
        
        await telegramBot.SendTextMessageAsync(notification.UpdatedBy, $"Bingo \"{notification.Bingo}\" has been updated",
            buttons:
            [
                TelegramButton.ShowAQuestionButton(notification.BingoId),
                TelegramButton.SkipButton("OK")
            ],
            cancellationToken: cancellationToken);
    }
}