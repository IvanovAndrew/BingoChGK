using Bingo.Application.Services;
using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using static System.Environment;

namespace Bingo.Application.GetBingoDescription;

public class GetBingoDescriptionCommandHandler(
    IBingoLookupService bingoLookupService,
    IUserRepository userRepository,
    IMediator mediator,
    ILogger<GetBingoDescriptionCommandHandler> logger)
    : IRequestHandler<GetBingoDescriptionCommand>
{
    public async Task Handle(GetBingoDescriptionCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Started. Command: {@Command}", command);
        
        var lookupResult = await bingoLookupService.LookupAsync(command.BingoId, command.Text, cancellationToken);
        
        if (lookupResult.Bingo is {} bingo)
        {
            await userRepository.SaveSession(command.ChatId, bingo.Id, DateTime.UtcNow);

            var description = BuildDescription(bingo);

            if (!string.IsNullOrWhiteSpace(description))
            {
                await mediator.Publish(new DescriptionFoundEvent(command.ChatId, bingo.Id, description), cancellationToken);
            }
            else
            {
                await mediator.Publish(new DescriptionNotFoundEvent()
                    { ChatId = command.ChatId, BingoId = bingo.Id, BingoText = bingo.Text }, cancellationToken);
            }
        }
        else
        {
            logger.LogInformation("Bingo is null");
            await mediator.Publish(new BingoNotFoundEvent() { ChatId = command.ChatId, BingoText = command.Text, ReplyTo = command.MessageId }, cancellationToken);
        }
        
        logger.LogInformation("Finished");
    }

    private static string BuildDescription(Domain.Bingo bingo)
    {
        var parts = new List<string> { bingo.Text, bingo.Description, bingo.InstantViewUrl }
            .Where(s => !string.IsNullOrWhiteSpace(s));

        return string.Join(NewLine, parts);
    }
}