using Bingo.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

public class QuestionsCacheDecorator(IQuestionSearcher searcher, ILogger logger) : IQuestionSearcher
{
    public async Task<List<Question>> GetQuestions(string word, DateOnly lastUpdate)
    {
        if (!Cache.Instance.TryGetValue(word, out List<Question> questions) || questions.Count == 0)
        {
            logger.LogInformation($"Cache miss for word {word}");
            questions = await searcher.GetQuestions(word, lastUpdate);
            Cache.Instance.Set(word, questions, TimeSpan.FromMinutes(15));
            logger.LogInformation($"Cache was updated. It contains {Cache.Instance.Count} element(s) now");
        }
        else
        {
            logger.LogInformation($"Cache contains {questions.Count} questions for {word}");
        }

        return questions;
    }

    public async Task<List<Question>> GetQuestions(IReadOnlyList<int> ids)
    {
        var result = new List<Question>();

        foreach (var questionId in ids)
        {
            var question = await GetQuestionById(questionId);

            if (question != null)
            {
                result.Add(question);
            }
        }

        return result;
    }

    public async Task<Question?> GetQuestionById(int id)
    {
        var cacheKey = $"Q{id}";
        if (Cache.Instance.TryGetValue(cacheKey, out Question cachedQuestion))
        {
            return cachedQuestion;
        }
        
        var question = await searcher.GetQuestionById(id);

        if (question != null)
        {
            Cache.Instance.Set(cacheKey, question);

            return question;
        }

        return null;
    }
}