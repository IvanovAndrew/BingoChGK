using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.DeleteQuestion;

public class DeleteQuestionCommandHandler(
    IBingoRepository bingoRepository,
    IQuestionRepository questionRepository,
    IMediator mediator,
    ILogger<DeleteQuestionCommandHandler> logger)
    : IRequestHandler<DeleteQuestionCommand>
{
    private readonly ILogger _logger = logger;

    public async Task Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(DeleteQuestionCommandHandler));

        var bingo = await bingoRepository.GetBingoByID(command.BingoID);
        if (bingo == null)
        {
            _logger.LogWarning($"Bingo {command.BingoID} hasn't been found");
            return;
        }
        
        var deleted = bingo.DeleteQuestion(command.QuestionID, command.ChatId);
        if (deleted)
        {
            await questionRepository.DeleteQuestion(command.QuestionID);
            _logger.LogInformation($"Question {command.QuestionID} has been removed");
        }
        else
        {
            _logger.LogWarning($"Question {command.QuestionID} not found in Bingo {command.BingoID}. Deletion failed.");
        }

        foreach (var domainEvent in bingo.GetEvents())
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }
        
        bingo.ClearEvents();
    }
}