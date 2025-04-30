using Bingo.Domain;
using MediatR;

namespace Bingo.Application.DeleteQuestion;

public class QuestionRemovedDomainEventHandler(ITelegramBot telegramBot) : INotificationHandler<QuestionRemovedDomainEvent>
{
    public async Task Handle(QuestionRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        string text = "The question has been deleted\\. Continue?";

        var sentMessage = await telegramBot.SendTextMessageAsync(notification.DeletedBy, text, useMarkdown:true, buttons: 
            new TelegramButton[]
            {
                new(){Text = "Yes", Callback = $"/question {notification.BingoId}"},
                new(){Text = "No", Callback = $"/skip"},
            }, cancellationToken: cancellationToken);
    }
}