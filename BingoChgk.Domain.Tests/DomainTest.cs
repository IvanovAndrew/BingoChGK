using Bingo.Domain;

namespace BingoChgk.Domain.Tests;

public class DomainTest
{
    [Fact]
    public void BingoQuestionsDoNotRepeatThemselves()
    {
        var user = new User(1, true, false);

        var userSession = new UserTrainingSession(user, new Dictionary<int, DateTime>());

        var questions = new List<Question>()
        {
            new Question(){GotQuestionId = 1, Text = "Q1", Answer = "A1"},
            new Question(){GotQuestionId = 2, Text = "Q2", Answer = "A2"},
            new Question(){GotQuestionId = 3, Text = "Q3", Answer = "A3"},
        };
        
        var q1 = userSession.GetBingoQuestion(10, questions);
        var q2 = userSession.GetBingoQuestion(10, questions);
        var q3 = userSession.GetBingoQuestion(10, questions);
        
        Assert.Equivalent(questions, new List<Question>(){q1, q2, q3});
    }
    
    [Fact]
    public void WhenAllBingoQuestionsAreShown_RepeatThem()
    {
        var user = new User(1, true, false);

        var userSession = new UserTrainingSession(user, new Dictionary<int, DateTime>());

        var questions = new List<Question>()
        {
            new Question(){GotQuestionId = 1, Text = "Q1", Answer = "A1"},
            new Question(){GotQuestionId = 2, Text = "Q2", Answer = "A2"},
        };
        
        var q1 = userSession.GetBingoQuestion(10, questions);
        var q2 = userSession.GetBingoQuestion(10, questions);
        var q3 = userSession.GetBingoQuestion(10, questions);
        
        Assert.NotNull(q3);
    }
}