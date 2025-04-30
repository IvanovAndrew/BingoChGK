using Bingo.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

public class QuestionsCacheDecorator(IQuestionSearcher searcher, ILogger logger) : IQuestionSearcher
{
    public async Task<List<Question>> GetQuestions(string word, int bingoId, DateOnly lastUpdate)
    {
        if (!Cache.Instance.TryGetValue(word, out List<Question> questions) || questions.Count == 0)
        {
            logger.LogInformation($"Cache miss for word {word}");
            questions = await searcher.GetQuestions(word, bingoId);
            Cache.Instance.Set(word, questions, TimeSpan.FromMinutes(15));
            logger.LogInformation($"Cache was updated. It contains {Cache.Instance.Count} element(s) now");
        }
        else
        {
            logger.LogInformation($"Cache contains {questions.Count} questions for {word}");
        }

        return questions;
    }
}