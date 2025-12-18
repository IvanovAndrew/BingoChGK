using Bingo.Application.FetchNewQuestions;
using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.GetBingoQuestion;

public class GetBingoQuestionCommandHandler(
    BingoQuestionService bingoQuestionService,
    IUserRepository userRepository,
    IMediator mediator,
    ILogger<GetBingoQuestionCommandHandler> logger)
    : IRequestHandler<GetBingoQuestionCommand>
{
    public async Task Handle(GetBingoQuestionCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Started {command}");

        var trainingSession = await userRepository.GetUserSession(command.ChatId);
        var questions = await bingoQuestionService.GetQuestionsForBingoAsync(command.BingoId);

        if (!questions.Any())
        {
            if (command.RetryIfNoQuestions)
            {
                await mediator.Send(
                    new FetchNewQuestionsCommand { BingoId = command.BingoId },
                    cancellationToken);

                await mediator.Send(
                    command with { RetryIfNoQuestions = false },
                    cancellationToken);

                return;
            }

            await mediator.Publish(
                new NoBingoQuestionsEvent(){ ChatId = command.ChatId, BingoId = command.BingoId},
                cancellationToken);

            return;
        }

        var question = trainingSession.GetBingoQuestion(command.BingoId, questions);

        await mediator.Publish(
            new BingoQuestionPreparedEvent(
                command.ChatId,
                command.BingoId,
                question,
                trainingSession.User.CanRemoveQuestion),
            cancellationToken);
    }
}