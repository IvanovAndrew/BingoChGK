using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.EditBingoDescription;

public class EditBingoDescriptionCommandHandler(
    IBingoRepository bingoRepository,
    IUnitOfWork unitOfWork,
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
        await unitOfWork.CommitAsync(cancellationToken);
    }
}