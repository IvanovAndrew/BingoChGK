using Bingo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bingo.Application.FetchNewQuestions;

public record FetchNewQuestionsCommand : IRequest
{
    public int BingoId { get; init; }
}

public class FetchNewQuestionsCommandHandler(IBingoRepository bingoRepository, IQuestionService questionService, BingoQuestionService bingoQuestionService, ILogger<FetchNewQuestionsCommandHandler> logger) : IRequestHandler<FetchNewQuestionsCommand>
{
    public async Task Handle(FetchNewQuestionsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching new questions for bingo ID: {BingoId}", request.BingoId);

        var bingo = await bingoRepository.GetBingoByID(request.BingoId);
        if (bingo == null)
        {
            logger.LogWarning("Bingo with ID {BingoId} not found", request.BingoId);
            return;
        }

        var existingQuestions = await questionService.GetQuestions(bingo.Id);
        var latestDate = existingQuestions.Any()
            ? existingQuestions.Max(q => q.Date)
            : DateOnly.MinValue;

        logger.LogInformation("Latest known question date for bingo {BingoId} is {Date}", bingo.Id, latestDate);
        
        var newQuestions = await questionService.GetNewQuestions(bingo.Text, latestDate);
        
        var synonyms = await bingoRepository.GetBingoSynonyms(bingo.Id);

        foreach (var synonym in synonyms)
        {
            var synonymQuestions = await questionService.GetNewQuestions(synonym, latestDate);
            newQuestions.AddRange(synonymQuestions);
        }

        newQuestions = MergeQuestions(newQuestions);
        logger.LogInformation("{Count} new question(s) found for bingo {BingoId}", newQuestions.Count, bingo.Id);

        if (newQuestions.Count > 0)
        {
            await questionService.LinkQuestionsToBingo(newQuestions, bingo.Id);
            logger.LogInformation("New questions inserted for bingo {BingoId}", bingo.Id);
            bingoQuestionService.Invalidate(bingo.Id);
        }

        logger.LogInformation("Finished fetching new questions for bingo {BingoId}", bingo.Id);
    }

    private List<Question> MergeQuestions(List<Question> newQuestions)
    {
        var dict = new Dictionary<int, Question>();

        foreach (var question in newQuestions)
        {
            if (!dict.TryAdd(question.GotQuestionId, question))
                continue;
        }
        
        return dict.Values.ToList();
    }
}