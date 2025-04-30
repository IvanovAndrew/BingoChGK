using Bingo.Domain;
using Microsoft.Extensions.Logging;

namespace Bingo.Infrastructure;

public class QuestionDBDecorator : IQuestionSearcher
{
    private readonly IQuestionSearcher _searcher;
    private readonly IBingoRepository _bingoRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger _logger;

    public QuestionDBDecorator(IQuestionSearcher searcher, IBingoRepository bingoRepository, IQuestionRepository questionRepository, ILogger<QuestionDBDecorator> logger)
    {
        _searcher = searcher;
        _bingoRepository = bingoRepository;
        _questionRepository = questionRepository;
        _logger = logger;
    }

    public async Task<List<Question>> GetQuestions(string word, int bingoId, DateOnly lastUpdate)
    {
        _logger.LogInformation(nameof(QuestionDBDecorator));
        
        var bingo = await _bingoRepository.GetBingoByText(word);
        
        _logger.LogInformation($"Bingo {word} has id {bingo.Id}");
        
        var dbQuestions = await _questionRepository.GetQuestions(bingo.Id);

        if (dbQuestions.Count > 0)
        {
            _logger.LogInformation($"There is/are {dbQuestions.Count} question(s) for {word} in the database");
            
            // looking for new questions

            var lastQuestionDate = dbQuestions.Max(q => q.Date);

            if (lastQuestionDate != DateOnly.FromDateTime(DateTime.Today))
            {
                var newQuestions = await _searcher.GetQuestions(word, bingo.Id, lastQuestionDate);
                _logger.LogInformation($"{newQuestions.Count} new question(s) was/were found in the ChGK db for word {word}");

                if (newQuestions.Count > 0)
                {
                    await _questionRepository.InsertQuestions(newQuestions);
                    dbQuestions = await _questionRepository.GetQuestions(bingo.Id);
                    _logger.LogInformation($"There is/are {dbQuestions.Count} question(s) for {word} in the database");
                }
            }

            return dbQuestions;
        }

        _logger.LogInformation($"There are no questions for {word} in the local database");

        var questions = await _searcher.GetQuestions(word, bingo.Id);

        if (questions.Count > 0)
        {
            _logger.LogInformation($"There is/are {questions.Count} question(s) in the ChGK db for word {word}");

            await _questionRepository.InsertQuestions(questions);

            questions = await _questionRepository.GetQuestions(bingo.Id);
            _logger.LogInformation($"There is/are {questions.Count} question(s) saved to the local db for word {word}");
            
            if (questions.Any())
            {
                _logger.LogInformation($"{questions.Count} questions were added to the database for {word}");
            }
        }
        else
        {
            _logger.LogInformation($"There are no questions in the ChGK db for word {word}");
        }

        return questions;
    }
}