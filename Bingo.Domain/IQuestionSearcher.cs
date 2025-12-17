namespace Bingo.Domain;

public interface IQuestionSearcher
{
    Task<List<Question>> GetQuestions(string word, DateOnly lastUpdate = default);
    Task<List<Question>> GetQuestions(IReadOnlyList<int> ids);
    Task<Question?> GetQuestionById(int id);
}