namespace Bingo.Domain;

public interface IQuestionRepository
{
    Task<List<int>> GetQuestions(int bingoId);
    ValueTask InsertQuestionsToBingo(List<Question> questionsToAdd, int bingoId);
    ValueTask<bool> DeleteQuestionFromBingo(int questionId, int bingoId);
}