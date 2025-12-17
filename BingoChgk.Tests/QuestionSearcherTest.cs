using Bingo.Domain;
using Bingo.Infrastructure;
using Microsoft.Extensions.Logging;
using Assert = Xunit.Assert;

namespace BingoChgk.Tests;

public class QuestionSearcherTest
{
    [Fact]
    public async Task CheckCamelotBingo()
    {
        var questionSearcher = new QuestionSearcher(new LoggerStub<QuestionSearcher>());
    
        var questions = await questionSearcher.GetQuestions("Камелот");
        
        Assert.Equal(18, questions.Count);
        Assert.Equal(18, questions.Count(c => !string.IsNullOrEmpty(c.Answer)));
        Assert.Equal(18, questions.Count(c => !string.IsNullOrEmpty(c.Text)));
        Assert.Equal(18, questions.Count(c => c.Number != 0));
        Assert.Equal(18, questions.Count(c => !string.IsNullOrEmpty(c.PackTitle)));
    }

    [Fact]
    public async Task CheckPenClubBingo()
    {
        var questionSearcher = new QuestionSearcher(new LoggerStub<QuestionSearcher>());
    
        var questions = await questionSearcher.GetQuestions("ПЕН-клуб");
        
        Assert.Equal(47, questions.Count);
        Assert.Equal(47, questions.Count(c => !string.IsNullOrEmpty(c.Answer)));
        Assert.Equal(47, questions.Count(c => !string.IsNullOrEmpty(c.Text)));
        Assert.Equal(47, questions.Count(c => c.Number != 0));
        Assert.Equal(47, questions.Count(c => !string.IsNullOrEmpty(c.PackTitle)));
    }
    
    [Fact]
    public async Task Check4ClassicalRomans()
    {
        var questionSearcher = new QuestionSearcher(new LoggerStub<QuestionSearcher>());
    
        var allQuestions = new List<List<Question>>
        {
            await questionSearcher.GetQuestions("Четыре классических романа"),
            await questionSearcher.GetQuestions("Троецарствие"),
            await questionSearcher.GetQuestions("Путешествие на Запад"),
            await questionSearcher.GetQuestions("Речные заводи"),
            await questionSearcher.GetQuestions("Сон в красном тереме")
        };

        var t = new HashSet<int>();
        foreach (var questions in allQuestions)
        {
            foreach (var question in questions) t.Add(question.GotQuestionId);
        }
        
        Assert.Equal(53, t.Count);
    }

    internal class LoggerStub<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            return;
        }
    }
}