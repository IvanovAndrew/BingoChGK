using Bingo.Domain;
using Bingo.Infrastructure.Database;
using Microsoft.Extensions.Logging;
using Supabase;

namespace Bingo.Infrastructure;

public class QuestionRepository(Client db, ILogger<QuestionRepository> logger) : IQuestionRepository
{
    public async Task<List<int>> GetQuestions(int bingoId)
    {
        var questionFromDb =  await db.From<BingoQuestionLinkDb>()
            .Where(q => q.BingoId == bingoId)
            .Get();
            
        return questionFromDb.Models
            .Select(_ => _.GotQuestionId)
            .ToList();
    }

    public async ValueTask InsertQuestionsToBingo(List<Question> questionsToAdd, int bingoId)
    {
        await db.From<BingoQuestionLinkDb>().Insert(questionsToAdd.Select(x => new BingoQuestionLinkDb(){BingoId = bingoId, GotQuestionId = x.GotQuestionId}).ToList());
    }

    public async ValueTask<bool> DeleteQuestionFromBingo(int questionId, int bingoId)
    {
        if (questionId > 0)
        {
            await db.From<BingoQuestionLinkDb>().Where(q => q.BingoId == bingoId && q.GotQuestionId == questionId).Delete();
            return true;
        }

        return false;
    }
}