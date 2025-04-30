using Bingo.Infrastructure;
using Microsoft.Extensions.Logging;
using Xunit;
using Assert = Xunit.Assert;

namespace BingoChgk.Tests;

public class Tests
{
    [Fact]
    public async Task CheckCamelotBingo()
    {
        var questionSearcher = new QuestionSearcher(new LoggerStub<QuestionSearcher>());
    
        var questions = await questionSearcher.GetQuestions("Камелот", 1);
        
        Assert.Equal(25, questions.Count);
        Assert.Equal(25, questions.Count(c => !string.IsNullOrEmpty(c.Answer)));
        Assert.Equal(25, questions.Count(c => !string.IsNullOrEmpty(c.Text)));
        Assert.Equal(25, questions.Count(c => c.Number != 0));
        Assert.Equal(25, questions.Count(c => !string.IsNullOrEmpty(c.PackTitle)));
    }

    private class LoggerStub<T> : ILogger<T>
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