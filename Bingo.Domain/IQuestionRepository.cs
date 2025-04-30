namespace Bingo.Domain;

public interface IQuestionRepository
{
    Task<List<Question>> GetQuestions(int bingoId);
    Task<Question?> GetQuestionById(int questionID);
    ValueTask InsertQuestions(List<Question> questionsToAdd);
    ValueTask<bool> DeleteQuestion(int questionId);
}