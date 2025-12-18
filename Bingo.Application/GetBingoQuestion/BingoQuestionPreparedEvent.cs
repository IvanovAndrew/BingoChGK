using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.GetBingoQuestion;

public record BingoQuestionPreparedEvent(
    long ChatId,
    int BingoId,
    Question Question,
    bool CanRemoveQuestion
) : INotification;

public class SendBingoQuestionToTelegramHandler(
    ITelegramBot telegramBot,
    ILogger<SendBingoQuestionToTelegramHandler> logger)
    : INotificationHandler<BingoQuestionPreparedEvent>
{
    public async Task Handle(
        BingoQuestionPreparedEvent notification,
        CancellationToken cancellationToken)
    {
        var format = QuestionFormatter.FormatQuestion(notification.Question);

        var buttons = notification.CanRemoveQuestion
            ? new[]
            {
                TelegramButton.ShowAQuestionButton(notification.BingoId, "Next question"),
                TelegramButton.DeleteQuestionButton(notification.BingoId, notification.Question.GotQuestionId, "Delete"),
                TelegramButton.ShowRandomBingoButton("Next bingo"),
            }
            : new[]
            {
                TelegramButton.ShowAQuestionButton(notification.BingoId, "Next question"),
                TelegramButton.ShowRandomBingoButton("Next bingo"),
            };

        if (!string.IsNullOrEmpty(notification.Question.AdditionalMaterialPictureUrl))
        {
            await telegramBot.SendPhotoAsync(
                notification.ChatId,
                new Uri(notification.Question.AdditionalMaterialPictureUrl),
                caption: format,
                useMarkdown: true,
                buttons: buttons,
                cancellationToken: cancellationToken);
        }
        else
        {
            await telegramBot.SendTextMessageAsync(
                notification.ChatId,
                format,
                useMarkdown: true,
                buttons: buttons,
                cancellationToken: cancellationToken);
        }

        logger.LogInformation(
            "Sent question {QuestionId}",
            notification.Question.GotQuestionId);
    }
}