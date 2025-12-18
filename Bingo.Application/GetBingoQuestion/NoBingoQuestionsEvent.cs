using MediatR;

namespace Bingo.Application.GetBingoQuestion;

public record NoBingoQuestionsEvent : INotification
{
    public long ChatId { get; init; }
    public int BingoId { get; init; }
}

public class NoBingoQuestionsEventHandler(ITelegramBot telegramBot) : INotificationHandler<NoBingoQuestionsEvent>
{
    public async Task Handle(NoBingoQuestionsEvent notification, CancellationToken cancellationToken)
    {
        await telegramBot.SendTextMessageAsync(notification.ChatId, "There are no questions for the bingo", cancellationToken: cancellationToken);
    }
}