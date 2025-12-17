using Bingo.Domain;

namespace Bingo.Application;

public class BingoQuestionService(IQuestionService questionService)
{
    public async Task<List<Question>> GetQuestionsForBingoAsync(int bingoId)
    {
        return await questionService.GetQuestions(bingoId);
    }
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