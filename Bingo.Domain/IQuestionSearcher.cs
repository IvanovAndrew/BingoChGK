namespace Bingo.Domain;

public interface IQuestionSearcher
{
    Task<List<Question>> GetQuestions(string word, int bingoId, DateOnly lastUpdate = default);
}