using Bingo.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace Bingo.Application;

public class BingoQuestionService(IQuestionService questionService, IMemoryCache cache)
{
    private static string CacheKey(int bingoId) => $"bingo-questions:{bingoId}";
    
    public async Task<List<Question>> GetQuestionsForBingoAsync(int bingoId)
    {
        return await cache.GetOrCreateAsync(
                   CacheKey(bingoId),
                   async entry =>
                   {
                       entry.AbsoluteExpirationRelativeToNow =
                           TimeSpan.FromMinutes(10);

                       return await questionService.GetQuestions(bingoId);
                   })
               ?? new List<Question>();
    }
    
    public void Invalidate(int bingoId) => cache.Remove(CacheKey(bingoId));
}

public interface IQuestionService
{
    Task<List<Question>> GetQuestions(int bingoId);
    Task<List<Question>> GetNewQuestions(string word, DateOnly dateSince);
    Task LinkQuestionsToBingo(List<Question> newQuestions, int bingoId);
}

public class QuestionService(IQuestionSearcher questionSearcher, IQuestionRepository questionRepository) : IQuestionService
{
    public async Task<List<Question>> GetQuestions(int bingoId)
    {
        var questionIds = await questionRepository.GetQuestions(bingoId);

        return await questionSearcher.GetQuestions(questionIds);
    }

    public async Task<List<Question>> GetNewQuestions(string word, DateOnly dateSince)
    {
        return await questionSearcher.GetQuestions(word, dateSince);
    }

    public async Task LinkQuestionsToBingo(List<Question> newQuestions, int bingoId)
    {
        await questionRepository.InsertQuestionsToBingo(newQuestions, bingoId);
    }
}