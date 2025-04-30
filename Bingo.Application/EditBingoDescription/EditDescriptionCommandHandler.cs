using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.EditBingoDescription;

public class EditBingoDescriptionCommandHandler(
    IBingoRepository bingoRepository,
    IMediator mediator,
    ILogger<EditBingoDescriptionCommand> logger)
    : IRequestHandler<EditBingoDescriptionCommand>
{
    public async Task Handle(EditBingoDescriptionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(nameof(EditBingoDescriptionCommand));
        logger.LogInformation($"Chat id {request.ChatId}");
        
        var bingo = await bingoRepository.GetBingoByID(request.BingoId);
        bingo.UpdateDescription(request.Description, request.ChatId);
        
        await bingoRepository.UpdateBingo(bingo);

        foreach (var domainEvent in bingo.GetEvents())
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        bingo.ClearEvents();
    }
}