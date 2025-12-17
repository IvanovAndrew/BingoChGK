using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.DeleteQuestion;

public class DeleteQuestionCommandHandler(
    IQuestionRepository questionRepository,
    IMediator mediator,
    ILogger<DeleteQuestionCommandHandler> logger)
    : IRequestHandler<DeleteQuestionCommand>
{
    private readonly ILogger _logger = logger;

    public async Task Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Started {command}");

        var deleted = await questionRepository.DeleteQuestionFromBingo(command.QuestionID, command.BingoID);

        if (deleted)
        {
            _logger.LogInformation("Question {QuestionId} removed from Bingo {BingoId}", command.QuestionID, command.BingoID);

            var domainEvent = new QuestionRemovedDomainEvent
            {
                BingoId = command.BingoID,
                QuestionId = command.QuestionID,
                DeletedBy = command.ChatId
            };

            await mediator.Publish(domainEvent, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Question {QuestionId} not found in Bingo {BingoId}.", command.QuestionID, command.BingoID);
        }
    }
}