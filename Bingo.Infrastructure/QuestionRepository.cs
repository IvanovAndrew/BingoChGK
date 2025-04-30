using Bingo.Domain;
using Bingo.Infrastructure.Database;

namespace Bingo.Infrastructure;

public class QuestionRepository(Supabase.Client db) : IQuestionRepository
{
    public async Task<List<Question>> GetQuestions(int bingoId)
    {
        var questionFromDb =  await db.From<QuestionDb>()
            .Where(q => q.BingoId == bingoId)
            .Get();
            
        return questionFromDb.Models
            .Select(QuestionMapper.ToDomain)
            .ToList();
    }

    public async Task<Question?> GetQuestionById(int questionID)
    {
        var dbResponse =  await db.From<QuestionDb>()
            .Where(q => q.Id == questionID)
            .Get();
        
        if (dbResponse.Model != null)
        {
            return QuestionMapper.ToDomain(dbResponse.Model);
        }
        return null;
    }

    public async ValueTask InsertQuestions(List<Question> questionsToAdd)
    {
        await db.From<QuestionDb>().Insert(questionsToAdd.Select(QuestionMapper.FromDomain).ToList());
    }

    public async ValueTask<bool> DeleteQuestion(int questionId)
    {
        if (questionId > 0)
        {
            await db.From<QuestionDb>().Where(q => q.Id == questionId).Delete();
            return true;
        }

        return false;
    }
}