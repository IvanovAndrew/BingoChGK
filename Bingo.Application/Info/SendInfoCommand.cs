using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.Info;

public record SendInfoCommand : IRequest
{
    public long ChatId { get; init; }
}

public class SendInfoCommandHandler(ITelegramBot telegramBot, ILogger<SendInfoCommandHandler> logger) : IRequestHandler<SendInfoCommand>
{
    public async Task Handle(SendInfoCommand request, CancellationToken cancellationToken)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("The list of available commands:");
        stringBuilder.AppendLine();
        
        foreach (var command in TelegramConstants.Commands)
        {
            stringBuilder.AppendLine($"- {command.Key} {command.Value}");
        }

        await telegramBot.SendTextMessageAsync(request.ChatId, stringBuilder.ToString(), cancellationToken: cancellationToken);
    }
}