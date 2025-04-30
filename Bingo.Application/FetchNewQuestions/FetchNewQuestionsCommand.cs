using Bingo.Domain;
using MediatR;

namespace Bingo.Application.FetchNewQuestions;

public record FetchNewQuestionsCommand(int bingoId) : IRequest
{
}

public class FetchNewQuestionsCommandHandler(IBingoRepository bingoRepository, IQuestionSearcher questionSearcher, IQuestionRepository questionRepository, IMediator mediator) : IRequestHandler<FetchNewQuestionsCommand>
{
    public async Task Handle(FetchNewQuestionsCommand request, CancellationToken cancellationToken)
    {
        var bingo = await bingoRepository.GetBingoByID(request.bingoId);

        var lastQuestionDay = bingo.LastQuestionDateOrDefault;

        var questions = await questionSearcher.GetQuestions(bingo.Text, bingo.Id, lastQuestionDay);
        
        bingo.AddQuestions(questions);

        await questionRepository.InsertQuestions(questions);

        foreach (var domainEvent in bingo.GetEvents())
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }
    }
}