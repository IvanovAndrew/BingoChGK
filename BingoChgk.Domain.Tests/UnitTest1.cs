using Bingo.Domain;

namespace BingoChgk.Domain.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var user = new User() { Id = 1, CanRemoveQuestion = false, CanAddBingo = false };

        var userSession = new UserTrainingSession(user, new Dictionary<int, DateTime>());

        var bingo = new BingoBuilder(10, "Bingo", "Bingo description").Build();
        var questions = new List<Question>()
        {
            new Question(){Id = 1, Text = "Q1",Answer = "A1", BingoId = 10},
            new Question(){Id = 2, Text = "Q2",Answer = "A2", BingoId = 10},
            new Question(){Id = 3, Text = "Q3",Answer = "A3", BingoId = 10},
        };
        
        bingo.AddQuestions(questions);

        var q1 = userSession.GetBingoQuestion(bingo);
        var q2 = userSession.GetBingoQuestion(bingo);
        var q3 = userSession.GetBingoQuestion(bingo);
        
        Assert.Equivalent(questions, new List<Question>(){q1, q2, q3});
    }
}